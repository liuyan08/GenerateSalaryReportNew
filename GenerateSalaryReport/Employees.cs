using System.Collections.Generic;

namespace GenerateSalaryReport
{
    public class EmployeeList 
    {
        public List<Employee> Employees { get; set; } 
    }

    public class Employee
    {
        public string userId { get; set; }
        public string jobTitleName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string preferredFullName { get; set; }
        public string employeeCode { get; set; }
        public string region { get; set; }
        public string phoneNumber { get; set; }
        public string emailAddress { get; set; }
        public string salary { get; set; }
    }

    public class SalaryRecord
    {
        public string region { get; set; }
        public string jobTitle { get; set; }
        public string averageSalary { get; set; }
    }
}
