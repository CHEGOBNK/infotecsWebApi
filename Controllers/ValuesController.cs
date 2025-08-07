using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper;
using infotecsWebApi.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Xml.Linq;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using CsvHelper.TypeConversion;
using Microsoft.EntityFrameworkCore;
using infotecsWebApi.Helpers;

namespace infotecsWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly IValidator<ValueDto> _validator;
        public ValuesController(ApplicationContext db, IValidator<ValueDto> validator)
        {
            _db = db;
            _validator = validator;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please provide a non-empty CSV file.");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = false,
                MissingFieldFound = null
            };

            var created = new List<Value>();
            var errors = new List<string>();

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<ValueDtoMap>();

            int row = 1;

            try
            {
                var lines = csv.GetRecords<ValueDto>().Take(10001).ToList();

                if (lines.Count > 10000)
                {
                    return BadRequest("File contains too many rows. Maximum is 10,000.");
                }


                foreach (var dto in lines)
                {
                    dto.FileName = file.FileName;



                    var result = _validator.Validate(dto);
                    if (!result.IsValid)
                    {
                        foreach (var f in result.Errors)
                            errors.Add($"Row {row}: '{f.PropertyName}' — {f.ErrorMessage}");
                    }
                    else
                    {
                        created.Add(new Value
                        {
                            FileName = dto.FileName,
                            Date = dto.Date,
                            ExecutionTime = TimeSpan.FromSeconds(dto.ExecutionTime),
                            ValueV = dto.ValueV
                        });
                    }

                    row++;

                    if (row > 10000)
                    {
                        errors.Add($"File contains too many rows.");
                    }
                }
            }
            catch (TypeConverterException)
            {
                errors.Add($"Row {row}: Incorrect data format.");
            }
            catch (Exception ex)
            {
                errors.Add($"Row {row}: Unexpected error: {ex.Message}");
            }

            if (errors.Any())
            {
                return BadRequest(new { Errors = errors });
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var existingValues = _db.Values.Where(v => v.FileName == file.FileName);
                var existingResults = _db.Results.Where(r => r.FileName == file.FileName);

                _db.Values.RemoveRange(existingValues);
                _db.Results.RemoveRange(existingResults);

                _db.Values.AddRange(created);

                var dates = created.Select(v => v.Date).ToList();
                var execTimes = created.Select(v => v.ExecutionTime).ToList();
                var values = created.Select(v => v.ValueV).OrderBy(v => v).ToList();

                var minDate = dates.Min();
                var maxDate = dates.Max();

                var result = new Result
                {
                    FileName = file.FileName,
                    MinimalDate = minDate,
                    DeltaDate = maxDate - minDate,
                    AverageExecutionTime = TimeSpan.FromMilliseconds(execTimes.Average(t => t.TotalMilliseconds)),
                    AverageValue = values.Average(),
                    MedianValue = Utils.GetMedian(values),
                    MaximalValue = values.Max(),
                    MinimalValue = values.Min()
                };

                _db.Results.Add(result);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    Count = created.Count,
                    FileName = file.FileName,
                    SummaryId = result.Id
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Error = $"Database error: {ex.Message}" });
            }
        }


        [HttpGet("results/filter")]
        public async Task<IActionResult> GetFilteredResults([FromQuery] ResultFilterDto filter)
        {
            var query = _db.Results.AsQueryable();

            if (!filter.HasAnyFilter())
            {
                return Ok(new List<Result>());
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(filter.FileName))
                    query = query.Where(r => r.FileName == filter.FileName);

                query = query
                    .WhereBetween(r => r.AverageValue, filter.MinAverageValue, filter.MaxAverageValue)
                    .WhereBetween(r => r.MinimalDate, filter.MinMinimalDate, filter.MaxMinimalDate)
                    .WhereBetween(r => r.AverageExecutionTime, filter.MinAverageExecutionTime, filter.MaxAverageExecutionTime);
            }

            var results = await query.ToListAsync();

            return Ok(results);
        }

        [HttpGet("values/last")]
        public async Task<IActionResult> GetLastValuesByFileName([FromQuery] string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("FileName is required.");

            var lastValues = await _db.Values
                .Where(v => v.FileName == fileName)
                .OrderByDescending(v => v.Date)
                .Take(10)
                .ToListAsync();

            return Ok(lastValues);
        }


        //Used for debug
        //[HttpPost]
        //public async Task<IActionResult> Create(ValueDto dto)
        //{
        //    var value = new Value
        //    {
        //        FileName = dto.FileName,
        //        Date = dto.Date,
        //        ExecutionTime = TimeSpan.FromSeconds(dto.ExecutionTime),
        //        ValueV = dto.ValueV
        //    };

        //    _db.Values.Add(value);
        //    await _db.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetById), new { id = value.Id }, value);
        //}

    }
}
