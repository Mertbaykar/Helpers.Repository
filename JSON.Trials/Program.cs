using JSON.Utils;
using Models;

string filepath = @"D:\Extension.Trials\Models\Company.json";
var employee = new Employee("Sami", "Çoker", 32);
//JsonFileUtils.AddObjectToRootModel<Company,Employee>(filepath, employee);
JsonFileUtils.AddValueToJsonFile<Company, string>(filepath, x => x.Name, "Merdoo");
