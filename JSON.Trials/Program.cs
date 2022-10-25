using JSON.Utils;
using Models;

string filepath = @"D:\Extension.Trials\Models\Company.json";
var employee = new Employee("Hüseyin", "Baykar", 67);
var employee2 = new Employee("Ezgi", "Baykar", 27);
//JsonFileUtils.AddValueToJsonFile<Company, Employee>(filepath, employee);
//JsonFileUtils.AddValueToJsonFile<Company, Employee>(filepath, x => x.Employees, new List<Employee>() { employee, employee2 });
//JsonFileUtils.AddValueToJsonFile<Company, Employee>(filepath, x => x.Boss, new Employee("Bülent", "Emet", 46));
//JsonFileUtils.AddValueToJsonFile<Company, Employee>(filepath, x => x.Employees, new List<Employee>() { employee, employee2 });
JsonFileUtils.AddValueToJsonFile<Company, Employee>(filepath, x => x.Boss.SubEmployees, new List<Employee>() { employee, employee2 });
