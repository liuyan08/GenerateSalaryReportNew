using System.IO;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using CsvHelper;
using System.Globalization;

namespace GenerateSalaryReport
{
    public class AverageSalaryReportGenerator
    {
        public static void GenerateAverageSalaryRecordToCsv(string filePathOfSalaryData, string filePahtOfReport)
        {
            var records = GenerateAverageSalaryRecord(filePathOfSalaryData);
            SaveAverageSalaryRecordToCSV(records, filePahtOfReport);
        }

        private static List<SalaryRecord> GenerateAverageSalaryRecord(string filePathOfSalaryData)
        {
            var jsonData = File.ReadAllText(filePathOfSalaryData);
            var employeeList = JsonSerializer.Deserialize<EmployeeList>(jsonData);

            //Clean data: remove duplicate records, records with invalid salary,i.e, negative values, empty, NULL, and string
            var distinctEmployeeList = employeeList.Employees.GroupBy(item => item.userId).Select(g => g.First());
            distinctEmployeeList = distinctEmployeeList.Where(item => !string.IsNullOrWhiteSpace(item.salary) &&  decimal.Parse(item.salary) > 0).ToList();

            foreach (var employ in distinctEmployeeList)
            {
                employ.region = employ.region.ToUpper();
                employ.jobTitleName = employ.jobTitleName.ToUpper();
            }

            var employeesGroupByRegionAndRole = distinctEmployeeList.GroupBy(employee => new { employee.region, employee.jobTitleName });

            var records = new List<SalaryRecord>();

            foreach (var group in employeesGroupByRegionAndRole)
            {
                var averSalary = group.Average(item => decimal.Parse(item.salary));
                records.Add(new SalaryRecord { region = group.Key.region, jobTitle = group.Key.jobTitleName, averageSalary = averSalary.ToString() });
            }

            return records;
        }

        private static void SaveAverageSalaryRecordToCSV(List<SalaryRecord> records, string filePahtOfReport)
        {
            using var writer = new StreamWriter(filePahtOfReport);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteHeader<SalaryRecord>();
            csv.NextRecord();
            foreach (var record in records)
            {
                csv.WriteRecord(record);
                csv.NextRecord();
            }
        }
    }
}
