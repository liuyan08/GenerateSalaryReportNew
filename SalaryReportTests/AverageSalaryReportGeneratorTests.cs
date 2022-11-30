using GenerateSalaryReport;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SalaryReportTests
{
    public class AverageSalaryReportGeneratorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        [Description("Duplicate Employee Records Test. " +
            "Assumption: if there are duplicated userId, only the first record will be used for salary caculation.")]
        public void ReadSalaryRecordFromCsv_DuplicateEmployeeRecords_DuplicateRecordsAreExcluded()
        {
            var mockEmployeeList = new EmployeeList
            {
                Employees = new List<Employee>
            {
                new Employee { region = "CA", userId = "1", jobTitleName = "Developer", salary = "15" },
                new Employee { region = "CA", userId = "1", jobTitleName = "Developer", salary = "10" },
                new Employee { region = "CA", userId = "2", jobTitleName = "ProductOwner", salary = "20" }
            }
            };
            var dataFilePath = @"./test/mock.json";
            SalaryReportTestHelper.GenerateMockJsonFile(mockEmployeeList, dataFilePath);

            var csvFilePath = @"./test/SaloryReport.csv";
            AverageSalaryReportGenerator.GenerateAverageSalaryRecordToCsv(dataFilePath, csvFilePath);

            var results = SalaryReportTestHelper.ReadSalaryRecordFromCsv(csvFilePath);

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "CA", jobTitle = "Developer", averageSalary = "15" })));
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "CA", jobTitle = "PRODUCTOWNER", averageSalary = "20" })));

            File.Delete(dataFilePath);
            File.Delete(csvFilePath);
        }

        [Test]
        [Description("If salary is invalid, i.e salary is a negative value, that record will not be used for salary caculation.")]
        public void ReadSalaryRecordFromCsv_NegativeSalary_RecordsAreExcludedFromResult()
        {
            var mockEmployeeList = new EmployeeList
            {
                Employees = new List<Employee>
                {
                    new Employee { region = "CA", userId = "1", jobTitleName = "Developer", salary = "-100" },
                    new Employee { region = "CA", userId = "2", jobTitleName = "Developer", salary = "10" },
                    new Employee { region = "CA", userId = "3", jobTitleName = "ProductOwner", salary = "20" }
                }
            };

            var dataFilePath = @"./test/mock.json";
            SalaryReportTestHelper.GenerateMockJsonFile(mockEmployeeList, dataFilePath);

            var csvFilePath = @"./test/SaloryReport.csv";
            AverageSalaryReportGenerator.GenerateAverageSalaryRecordToCsv(dataFilePath, csvFilePath);

            var results = SalaryReportTestHelper.ReadSalaryRecordFromCsv(csvFilePath);

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "CA", jobTitle = "DEVELOPER", averageSalary = "10" })));
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "CA", jobTitle = "PRODUCTOWNER", averageSalary = "20" })));

            File.Delete(dataFilePath);
            File.Delete(csvFilePath);
        }

        [Test]
        [Description("If salary is emtry or null, those records will not be used for salary caculation.")]
        public void ReadSalaryRecordFromCsv_EmptyAndNullSalary_RecordsAreExcludedFromResult()
        {
            var mockEmployeeList = new EmployeeList
            {
                Employees = new List<Employee>
                {
                    new Employee { region = "CA", userId = "1", jobTitleName = "Developer", salary = "" },
                    new Employee { region = "CA", userId = "2", jobTitleName = "Developer", },
                    new Employee { region = "CA", userId = "3", jobTitleName = "Developer", salary = "10" },
                    new Employee { region = "CA", userId = "4", jobTitleName = "Developer", salary = "20" }
                }
            };

            var dataFilePath = @"./test/mock.json";
            SalaryReportTestHelper.GenerateMockJsonFile(mockEmployeeList, dataFilePath);

            var csvFilePath = @"./test/SaloryReport.csv";
            AverageSalaryReportGenerator.GenerateAverageSalaryRecordToCsv(dataFilePath, csvFilePath);
            var results = SalaryReportTestHelper.ReadSalaryRecordFromCsv(csvFilePath);

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "CA", jobTitle = "DEVELOPER", averageSalary = "15" })));

            File.Delete(dataFilePath);
            File.Delete(csvFilePath);
        }

        [Test]
        [Description("When Json data for employees is empty, the application can work without a crash.")]
        public void ReadSalaryRecordFromCsv_EmptyEmployeeJsonData_ResultIsEmpty()
        {
            var mockEmployeeList = new EmployeeList
            {
                Employees = new List<Employee>()
            };

            var dataFilePath = @"./test/mock.json";
            SalaryReportTestHelper.GenerateMockJsonFile(mockEmployeeList, dataFilePath);

            var csvFilePath = @"./test/SaloryReport.csv";
            AverageSalaryReportGenerator.GenerateAverageSalaryRecordToCsv(dataFilePath, csvFilePath);

            var results = SalaryReportTestHelper.ReadSalaryRecordFromCsv(csvFilePath);

            Assert.AreEqual(0, results.Count);

            File.Delete(dataFilePath);
            File.Delete(csvFilePath);
        }

        [Test]
        [Description("Region  and job title are not case sensitive.")]
        public void ReadSalaryRecordFromCsv_RegionAndRoleNameMixedCases_CaseIsIgnored()
        {
            var mockEmployeeList = new EmployeeList
            {
                Employees = new List<Employee>
                {
                    new Employee { region = "CA", userId = "1", jobTitleName = "Developer", salary = "10" },
                    new Employee { region = "CA", userId = "2", jobTitleName = "developer", salary = "20" },
                    new Employee { region = "ca", userId = "3", jobTitleName = "Developer", salary = "30" }
                }
            };

            var dataFilePath = @"./test/mock.json";
            SalaryReportTestHelper.GenerateMockJsonFile(mockEmployeeList, dataFilePath);

            var csvFilePath = @"./test/SaloryReport.csv";
            AverageSalaryReportGenerator.GenerateAverageSalaryRecordToCsv(dataFilePath, csvFilePath);

            var results = SalaryReportTestHelper.ReadSalaryRecordFromCsv(csvFilePath);

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "CA", jobTitle = "DEVELOPER", averageSalary = "20" })));

            File.Delete(dataFilePath);
            File.Delete(csvFilePath);
        }


        [Test]
        [Description("Throw an assert if the perf has a regression")]
        public void ReadSalaryRecordFromCsv_MassData_PerformanceAsExpected()
        {
            var mockEmployeeList = new EmployeeList
            {
                Employees = new List<Employee>()
            };

            for (int i = 0; i < 1000; i++)
            {
                mockEmployeeList.Employees.Add(new Employee
                {
                    region = "CA",
                    userId = i.ToString(),
                    jobTitleName = "Developer",
                    salary = (i + 1000).ToString()
                });
            }


            var dataFilePath = @"./test/mock.json";
            SalaryReportTestHelper.GenerateMockJsonFile(mockEmployeeList, dataFilePath);

            //Start the timer when reading data from Json
            var timer = new Stopwatch();
            timer.Start();

            var csvFilePath = @"./test/SaloryReport.csv";
            AverageSalaryReportGenerator.GenerateAverageSalaryRecordToCsv(dataFilePath, csvFilePath);

            //Stop the timer after the report is generated to CSV.
            timer.Stop();

            File.Delete(dataFilePath);
            File.Delete(csvFilePath);

            var elapsedTime = timer.ElapsedMilliseconds;

            if (elapsedTime > 1000000)
            {
                throw new Exception("performance test failed");
            }
        }


        [Test]
        [Description("Verify Salary Report Generated from Sample Data")]
        public void ReadSalaryRecordFromCsv_ReportGeneratedFromSampleData_ReportAsExpected()
        {
            var csvFilePath = @"./test/SaloryReport.csv";
            AverageSalaryReportGenerator.GenerateAverageSalaryRecordToCsv(@"./test/Sample_Data.json", csvFilePath);

            var results = SalaryReportTestHelper.ReadSalaryRecordFromCsv(csvFilePath);

            Assert.AreEqual(7, results.Count);

            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "CA", jobTitle = "DEVELOPER", averageSalary = "25000" })));
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "CA", jobTitle = "PROGRAM DIRECTORY", averageSalary = "15000" })));
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "SA", jobTitle = "PRODUCTOWNER", averageSalary = "31666.666666666666666666666667" })));
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "FA", jobTitle = "DEVELOPER", averageSalary = "10000" })));
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "YA", jobTitle = "DEVELOPER", averageSalary = "20000" })));
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "YA", jobTitle = "PRODUCTOWNER", averageSalary = "17000" })));
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "CA", jobTitle = "PRODUCTOWNER", averageSalary = "2000" })));

            File.Delete(csvFilePath);
        }

        [Test]
        [Description("If salary cannot be converted to decimal value, those records will not be used for salary caculation.")]

        //This is a failed test, which is not fixed in the main program!!!
        public void ReadSalaryRecordFromCsv_StringSalary_RecordsAreExcludedFromResult()
        {
            var mockEmployeeList = new EmployeeList
            {
                Employees = new List<Employee>
                {
                    new Employee { region = "CA", userId = "1", jobTitleName = "Developer", salary = "abc" },
                    new Employee { region = "CA", userId = "2", jobTitleName = "Developer", salary = "20" }
                }
            };

            var dataFilePath = @"./test/mock.json";
            SalaryReportTestHelper.GenerateMockJsonFile(mockEmployeeList, dataFilePath);

            var csvFilePath = @"./test/SaloryReport.csv";
            AverageSalaryReportGenerator.GenerateAverageSalaryRecordToCsv(dataFilePath, csvFilePath);
            var results = SalaryReportTestHelper.ReadSalaryRecordFromCsv(csvFilePath);

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "CA", jobTitle = "DEVELOPER", averageSalary = "20" })));

            File.Delete(dataFilePath);
            File.Delete(csvFilePath);
        }

        [Test]
        [Description("Support naming Region code and job title in unicode, i.e Chinese characters")]
        public void ReadSalaryRecordFromCsv_RegionAndRoleNameInUnicode_ReportAsExpected()
        {
            var mockEmployeeList = new EmployeeList
            {
                Employees = new List<Employee>
                {
                    new Employee { region = "北京", userId = "1", jobTitleName = "开发", salary = "10" },
                    new Employee { region = "北京", userId = "2", jobTitleName = "开发", salary = "20" },
                    new Employee { region = "ca", userId = "3", jobTitleName = "Developer", salary = "30" }
                }
            };

            var dataFilePath = @"./test/mock.json";
            SalaryReportTestHelper.GenerateMockJsonFile(mockEmployeeList, dataFilePath);

            var csvFilePath = @"./test/SaloryReport.csv";
            AverageSalaryReportGenerator.GenerateAverageSalaryRecordToCsv(dataFilePath, csvFilePath);

            var results = SalaryReportTestHelper.ReadSalaryRecordFromCsv(csvFilePath);

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "CA", jobTitle = "DEVELOPER", averageSalary = "30" })));
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "北京", jobTitle = "开发", averageSalary = "15" })));

            File.Delete(dataFilePath);
            File.Delete(csvFilePath);
        }

        [Test]
        [Description("Records with empty job title should be removed without an exception")]
        //This is a failed test, which is not fixed in the main program!!!
        public void ReadSalaryRecordFromCsv_JobTitleIsNull_RecordsAreExludedFromResult()
        {
            var mockEmployeeList = new EmployeeList
            {
                Employees = new List<Employee>
                {
                    new Employee { region = "CA", userId = "1", salary = "10" },
                    new Employee { region = "CA", userId = "2", jobTitleName = "developer", salary = "20" },
                    new Employee { region = "ca", userId = "3", jobTitleName = "Developer", salary = "30" }
                }
            };

            var dataFilePath = @"./test/mock.json";
            SalaryReportTestHelper.GenerateMockJsonFile(mockEmployeeList, dataFilePath);

            var csvFilePath = @"./test/SaloryReport.csv";
            AverageSalaryReportGenerator.GenerateAverageSalaryRecordToCsv(dataFilePath, csvFilePath);

            var results = SalaryReportTestHelper.ReadSalaryRecordFromCsv(csvFilePath);

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "CA", jobTitle = "DEVELOPER", averageSalary = "25" })));

            File.Delete(dataFilePath);
            File.Delete(csvFilePath);
        }

        [Test]
        [Description("Records with empty region code should be removed without an exception")]

        //This is a failed test, which is not fixed in the main program!!!
        public void ReadSalaryRecordFromCsv_RegionIsNull_RecordsAreExcludedFromResult()
        {
            var mockEmployeeList = new EmployeeList
            {
                Employees = new List<Employee>
                {
                    new Employee { region = "", userId = "1", jobTitleName = "developer", salary = "10" },
                    new Employee { region = "CA", userId = "2", jobTitleName = "developer", salary = "20" },
                    new Employee { region = "ca", userId = "3", jobTitleName = "Developer", salary = "30" }
                }
            };

            var dataFilePath = @"./test/mock.json";
            SalaryReportTestHelper.GenerateMockJsonFile(mockEmployeeList, dataFilePath);

            var csvFilePath = @"./test/SaloryReport.csv";
            AverageSalaryReportGenerator.GenerateAverageSalaryRecordToCsv(dataFilePath, csvFilePath);

            var results = SalaryReportTestHelper.ReadSalaryRecordFromCsv(csvFilePath);

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(true, results.Any(x => AverageSalaryRecordsAreEqual(x, new SalaryRecord { region = "CA", jobTitle = "DEVELOPER", averageSalary = "25" })));

            File.Delete(dataFilePath);
            File.Delete(csvFilePath);
        }

        private bool AverageSalaryRecordsAreEqual(SalaryRecord a, SalaryRecord b)
        {
            return a.jobTitle.Equals(b.jobTitle, StringComparison.OrdinalIgnoreCase) &&
                a.region.Equals(b.region, StringComparison.OrdinalIgnoreCase) &&
                a.averageSalary.Equals(b.averageSalary);
        }
    }
}
