using Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace JSON.Utils
{
    public class JsonUtils
    {
        public static JsonSerializerOptions options => new JsonSerializerOptions()
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        /// <summary>
        /// Adds employee to company and saves in the specified json file
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="employee"></param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public static void AddEmployeeToCompany(string filepath, Employee employee)
        {

            #region File Validation
            if (!File.Exists(filepath))
                throw new FileNotFoundException();
            FileInfo file = new FileInfo(filepath);
            if (file.Extension is not ".json")
                throw new Exception("File must have json extension");
            #endregion


            try
            {
                #region Update data and write to specified file
                string json = "";

                using (StreamReader reader = new StreamReader(filepath))
                {
                    json = reader.ReadToEnd();
                }

                Company company = JsonSerializer.Deserialize<Company>(json);

                if (!company.Employees.Contains(employee))
                {
                    company.Employees.Add(employee);
                    json = JsonSerializer.Serialize(company, options);
                    File.WriteAllText(filepath, json);
                }
                
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

        }
    }
}