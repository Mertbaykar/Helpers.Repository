using JSON.Utils;
using Models;

string filepath = @"D:\Extension.Trials\Models\Company.json";
var employee = new Employee("Pelin", "Baykar", 39);
var employee2 = new Employee("Rüveyde", "Baykar", 62);
//JsonFileUtils.AddValueToJsonFile<Company, Employee>(filepath, employee);
JsonFileUtils.AddValueToJsonFile<Company, Employee>(filepath, x => x.Employees, new List<Employee>() { employee, employee2 });
