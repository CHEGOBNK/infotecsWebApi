using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace infotecsWebApi.Entities
{
    public class ValueDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTimeOffset Date { get; set; }
        public double ExecutionTime { get; set; }
        public float ValueV { get; set; }
    }

    public class ValuesDtoValidator : AbstractValidator<ValueDto>
    {
        public ValuesDtoValidator()
        {
            RuleFor(x => x.FileName).NotEmpty();
            RuleFor(x => x.ExecutionTime)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ExecutionTime must be ≥ 0");

            RuleFor(x => x.ValueV)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Value must be ≥ 0");

            RuleFor(x => x.Date)
                .GreaterThanOrEqualTo(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero))
                    .WithMessage("Timestamp can’t be before Jan 1, 2000")
                .LessThanOrEqualTo(DateTimeOffset.UtcNow)
                    .WithMessage("Timestamp can’t be in the future");
        }
    }
}
