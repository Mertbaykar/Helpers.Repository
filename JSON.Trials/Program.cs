using JSON.Utils;
using Models;

string filepath = @"P:\Training.Repository\Mertbaykar\Helpers.Repository\Models\Company.json";
var employee = new Employee("Pelin", "Baykar", 39);
var employee2 = new Employee("Mert", "Baykar", 24);
var employee3 = new Employee("Yasin", "Ceylan", 33);
var employee4 = new Employee("Bülent", "Emet", 46);
//JsonFileUtils.AddValueToJsonFile<Company, Employee>(filepath, employee);
//JsonFileUtils.AddValueToJsonFile<Company, Employee>(filepath, x => x.Employees, new List<Employee>() { employee, employee2 });
JsonFileUtils.AddValueToJsonFile<Company, Employee>(filepath, x => x.Employees, employee3);
//JsonFileUtils.AddValueToJsonFile<Company, Employee>(filepath, x => x.Employees, new List<Employee>() { employee, employee2 });
//JsonFileUtils.AddValueToJsonFile<Company, Employee>(filepath, x => x.Employees, new List<Employee>() { employee, employee2 });
//JsonFileUtils.AddValueToJsonFile<Company, Employee>(filepath, x => x.Boss.SubEmployees, new List<Employee>() { employee, employee2 });
