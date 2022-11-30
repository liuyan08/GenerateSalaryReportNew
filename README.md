1.Download GenerateSalaryReport and its child folders/files to your local.
2. Open \GenerateSalaryReport\GenerateSalaryReport.sln using visual Studio.
3. There are two projects under this solution, one is the main program "GenerateSalaryReport" used to read enmployee salary data from Json file and generate a salary report. 
The other one is the test project "SalaryReportTests".
4. For the main program, the output report "SaloryReport.csv" will be generated either under Debug path or Release path depending on it is running by release or debug.

\GenerateSalaryReport\GenerateSalaryReport\bin\Debug\netcoreapp3.1
\GenerateSalaryReport\GenerateSalaryReport\bin\Release\netcoreapp3.1

5. For SalaryReportTests project, AverageSalaryReportGeneratorTests.cs is the class for automated tests.
There is a "Test Exlporer" output window. Expand the tests under the test root, there are 11 automated tests.
You can choose the root and then run the tests by right-click.

These tests are end to end tests,which starts from reading data from Json and comparing the records from csv salary reports with the expected values.

There are 8 succeeful tests, 3 falure tests, which can prove the program still has the issues to be fixed.
