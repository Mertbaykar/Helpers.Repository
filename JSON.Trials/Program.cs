using JSON.Utils;
using Models;

string filepath = @"D:\Extension.Trials\Models\Company.json";
var employee = new Employee("Sami", "Çoker", 32);
JsonUtils.AddEmployeeToCompany(filepath, employee);
