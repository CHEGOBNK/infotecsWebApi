import csv
import random
from datetime import datetime, timezone, timedelta

def generate_test_csv(filename="test_data.csv", num_rows=10001):
    """
    Generate a CSV file with the specified number of rows for testing.
    Format: Date;ExecutionTime;ValueV
    """
    
    start_date = datetime(2000, 1, 1, tzinfo=timezone.utc)
    end_date = datetime.now(timezone.utc)
    date_range = (end_date - start_date).total_seconds()
    
    with open(filename, 'w', newline='', encoding='utf-8') as csvfile:
        writer = csv.writer(csvfile, delimiter=';')
        
        for i in range(num_rows):

            random_seconds = random.uniform(0, date_range)
            random_date = start_date + timedelta(seconds=random_seconds)
            date_str = random_date.strftime('%Y-%m-%dT%H:%M:%SZ')
            
            execution_time = round(random.uniform(0.1, 300.0), 1)
            
            value_v = round(random.uniform(0.1, 1000.0), 1)
            
            writer.writerow([date_str, execution_time, value_v])
            
            if (i + 1) % 1000 == 0:
                print(f"Generated {i + 1} rows...")
    
    print(f"Successfully generated {filename} with {num_rows} rows")

if __name__ == "__main__":
    generate_test_csv("test_10001_rows.csv", 10001)   
    print("\nFiles generated")