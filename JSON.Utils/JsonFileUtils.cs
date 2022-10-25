using Models;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace JSON.Utils
{
    public class JsonFileUtils
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
            ValidateFile(filepath);

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

        /// <summary>
        /// Finds property with type of TValue in TRoot and sets or adds value. If operation was a success, then writes JSON result to the specified file.
        /// Otherwise throws exception.
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="value"></param>
        public static void AddValueToJsonFile<TRoot, TValue>(string filepath, TValue value) where TRoot : class
        {
            ValidateValueType(typeof(TValue));
            TRoot rootData = GetData<TRoot>(filepath);
            AddData(rootData, value);
            WriteToFile(filepath, rootData);
        }

        public static void AddValueToJsonFile<TRoot, TValue>(string filepath, Expression<Func<TRoot, TValue>> propertyExpression, TValue value) where TRoot : class
        {
            ValidateValueType(typeof(TValue));
            TRoot rootData = GetData<TRoot>(filepath);
            AddData(rootData, propertyExpression, value);
            WriteToFile(filepath, rootData);
        }

        public static void AddValueToJsonFile<TRoot, TValue>(string filepath, Expression<Func<TRoot, ICollection<TValue>>> propertyExpression, TValue value) where TRoot : class
        {
            ValidateValueType(typeof(TValue));
            TRoot rootData = GetData<TRoot>(filepath);
            AddData(rootData, propertyExpression, value);
            WriteToFile(filepath, rootData);
        }

        public static void AddValueToJsonFile<TRoot, TValue>(string filepath, Expression<Func<TRoot, ICollection<TValue>>> propertyExpression, ICollection<TValue> values) where TRoot : class
        {
            ValidateValueType(typeof(TValue));
            TRoot rootData = GetData<TRoot>(filepath);
            AddData(rootData, propertyExpression, values);
            WriteToFile(filepath, rootData);
        }
        
        /// <summary>
        /// Finds the property of rootData with specified property name and sets value. If property not found, throws exception
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="rootData"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="value"></param>
        public static void AddData<TRoot, TValue>(TRoot rootData, Expression<Func<TRoot, TValue>> propertyExpression, TValue value) where TRoot : class
        {
            HandlePropertyValueByExpression(propertyExpression, value, rootData);
        }
        /// <summary>
        /// Finds the property of rootData with specified property name and adds value. If property not found, throws exception
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="rootData"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="values"></param>
        /// <exception cref="MissingMemberException"></exception>
        /// <exception cref="Exception"></exception>
        public static void AddData<TRoot, TValue>(TRoot rootData, Expression<Func<TRoot, ICollection<TValue>>> propertyExpression, ICollection<TValue> values) where TRoot : class
        {
            HandlePropertyValuesByExpression(propertyExpression, values, rootData);
            //if (property is null)
            //    throw new MissingMemberException(typeof(TRoot).Name, property.Name);

            //if (property.PropertyType == typeof(List<TValue>))
            //{
            //    List<TValue> list = property.GetValue(rootData) as List<TValue>;
            //    if (list == null)
            //        list = new List<TValue>();
            //    list.AddRange(values);
            //    property.SetValue(rootData, list);
            //}
            //else
            //{
            //    var typeName = typeof(TValue).Name;
            //    throw new Exception(property.Name + " property must be declared as List<" + typeName + "> or " + typeName);
            //}
        }
        /// <summary>
        /// Finds the property of rootData with specified property name and adds value. If property not found, throws exception
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="rootData"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="value"></param>
        public static void AddData<TRoot, TValue>(TRoot rootData, Expression<Func<TRoot, ICollection<TValue>>> propertyExpression, TValue value) where TRoot : class
        {
            //var property = HandlePropertyValueByExpression(propertyExpression,value);
            //AddData(rootData, propertyName, value);
        }
        
        /// <summary>
        /// Sets given value to the given property captured by expression
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <param name="rootData"></param>
        /// <exception cref="Exception"></exception>
        public static void HandlePropertyValueByExpression<TRoot, TValue>(Expression<Func<TRoot, TValue>> expression, TValue value, TRoot rootData)
        {
            try
            {
                LambdaExpression lambda = (LambdaExpression)expression;
                MemberExpression memberExpression;

                if (lambda.Body is UnaryExpression)
                {
                    UnaryExpression unaryExpression = (UnaryExpression)(lambda.Body);
                    memberExpression = (MemberExpression)(unaryExpression.Operand);
                }
                else
                    memberExpression = (MemberExpression)(lambda.Body);


                PropertyInfo parentProperty = null;

                try
                {
                    parentProperty = (memberExpression.Expression as MemberExpression).Member as PropertyInfo;
                }
                catch (Exception ex)
                {
                    parentProperty = memberExpression.Member as PropertyInfo;
                }

                var parentValue = parentProperty.GetValue(rootData);
                PropertyInfo property = memberExpression.Member as PropertyInfo;
                var searchedProperty = parentValue.GetType().GetProperty(property.Name);
                
                if (searchedProperty != null)
                {
                    searchedProperty.SetValue(parentValue, value);
                    parentProperty.SetValue(rootData, parentValue);
                }
                else
                {
                    parentProperty.SetValue(rootData, value);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        /// <summary>
        /// Adds collection to specified property which is known as collection
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression"></param>
        /// <param name="values"></param>
        /// <param name="rootData"></param>
        /// <exception cref="Exception"></exception>
        public static void HandlePropertyValuesByExpression<TRoot, TValue>(Expression<Func<TRoot, ICollection<TValue>>> expression, ICollection<TValue> values, TRoot rootData)
        {
            try
            {
                LambdaExpression lambda = (LambdaExpression)expression;
                MemberExpression memberExpression;

                if (lambda.Body is UnaryExpression)
                {
                    UnaryExpression unaryExpression = (UnaryExpression)(lambda.Body);
                    memberExpression = (MemberExpression)(unaryExpression.Operand);
                }
                else
                    memberExpression = (MemberExpression)(lambda.Body);

                PropertyInfo parentProperty = null;

                try
                {
                    parentProperty = (memberExpression.Expression as MemberExpression).Member as PropertyInfo;
                }
                catch (Exception)
                {
                    parentProperty = memberExpression.Member as PropertyInfo;
                }

                var parentValue = parentProperty.GetValue(rootData);
                PropertyInfo property = memberExpression.Member as PropertyInfo;
                var searchedProperty = parentValue.GetType().GetProperty(property.Name);

                if (searchedProperty!= null)
                {
                    var list = searchedProperty.GetValue(parentValue) as ICollection<TValue>;
                    if (list is null)
                        list = new List<TValue>();

                    foreach (TValue value in values)
                    {
                        list.Add(value);
                    }
                    searchedProperty.SetValue(parentValue, list);
                    parentProperty.SetValue(rootData, parentValue);
                }
                else
                {
                    var list = parentValue as ICollection<TValue>;
                    foreach (TValue value in values)
                    {
                        list.Add(value);
                    }
                    parentProperty.SetValue(rootData, list);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Ensures that the given type is derived from IConvertible interface or it is a custom written class
        /// </summary>
        /// <param name="valueType"></param>
        /// <exception cref="Exception"></exception>
        public static void ValidateValueType(Type valueType)
        {
            if (!(IsCustomClass(valueType) || valueType.IsAssignableTo(typeof(IConvertible))))
                throw new Exception("Value type must be a class or must be primitive");
        }
        /// <summary>
        /// Check whether the given type is a custom written class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCustomClass(Type type)
        {
            if (type.IsClass && !type.FullName.StartsWith("System."))
                return true;
            return false;
        }
        /// <summary>
        /// Finds property of TRoot which is type of TValue, if found, adds or sets object. Otherwise, throws exception
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="rootData"></param>
        /// <param name="value"></param>
        /// <exception cref="MissingMemberException"></exception>
        public static void AddData<TRoot, TValue>(TRoot rootData, TValue value) where TRoot : class
        {
            List<PropertyInfo> rootProperties = typeof(TRoot).GetProperties().ToList();
            bool isPropertyFound = false;

            foreach (PropertyInfo property in rootProperties)
            {
                if (property.PropertyType == typeof(TValue))
                {
                    property.SetValue(rootData, value);
                    isPropertyFound = true;
                    break;
                }
                else if (property.PropertyType.IsAssignableTo(typeof(List<TValue>)))
                {
                    List<TValue> list = property.GetValue(rootData) as List<TValue>;
                    if (list == null)
                        list = new List<TValue>();
                    list.Add(value);
                    property.SetValue(rootData, list);
                    isPropertyFound = true;
                    break;
                }
            }
            if (!isPropertyFound)
            {
                var typeName = typeof(TValue).Name;
                throw new MissingMemberException("Property of " + typeName + " type is not found in " + typeName + " type");

            }
        }
        /// <summary>
        /// Gets data from JSON file
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="filepath"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        public static TData GetData<TData>(string filepath) where TData : class
        {
            string json = ReadFromFile(filepath);
            TData data = null;

            try
            {
                data = JsonSerializer.Deserialize<TData>(json);
                return data;
            }
            catch (Exception ex)
            {
                throw new JsonException(ex.Message, ex);
            }
        }
        /// <summary>
        /// Ensures if the specified file exist and is JSON file. If so, reads JSON file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns>Contents of JSON file</returns>
        public static string ReadFromFile(string filepath)
        {
            ValidateFile(filepath);
            string json = "";

            using (StreamReader reader = new StreamReader(filepath))
            {
                json = reader.ReadToEnd();
            }
            return json;
        }

        /// <summary>
        /// Ensures if the specified file exist and is JSON file.
        /// </summary>
        /// <param name="filepath"></param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public static void ValidateFile(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException();
            FileInfo file = new FileInfo(filepath);
            if (file.Extension is not ".json")
                throw new Exception("File must have json extension");
        }
        /// <summary>
        /// Writes JSON result to specified file
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="data"></param>
        public static void WriteToFile<TData>(string filepath, TData data) where TData : class
        {
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filepath, json);
        }
    }
}