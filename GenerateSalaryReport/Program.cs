
namespace GenerateSalaryReport
{
    class Program
    {
        static void Main(string[] args)
        {
            AverageSalaryReportGenerator.GenerateAverageSalaryRecordToCsv(@"./Sample_Data.json", @"./SaloryReport.csv");
        }
    }
}
