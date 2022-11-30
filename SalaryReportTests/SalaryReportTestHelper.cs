using CsvHelper;
using CsvHelper.Configuration;
using GenerateSalaryReport;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SalaryReportTests
{
    class SalaryReportTestHelper
    {
        public static void GenerateMockJsonFile(EmployeeList employeeList, string filePath)
        {
            var json = JsonSerializer.Serialize(employeeList);
            File.WriteAllText(filePath, json);
        }

        public static List<SalaryRecord> ReadSalaryRecordFromCsv(string csvFilePath)
        {
            var records = new List<SalaryRecord>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," };

            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, config))
            {
                records = csv.GetRecords<SalaryRecord>().ToList();
            }

            return records;
        }

    }

}