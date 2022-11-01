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
        /// Reads specified JSON file and deserializes as TRoot type. Finds the specified property and sets the given value as property value, then overwrites the specified file by serializing and writing current TRoot data.
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="value"></param>
        public static void AddValueToJsonFile<TRoot, TValue>(string filepath, Expression<Func<TRoot, TValue>> propertyExpression, TValue value) where TRoot : class
        {
            ValidateValueType(typeof(TValue));
            TRoot rootData = GetData<TRoot>(filepath);
            AddData(rootData, propertyExpression, value);
            WriteToFile(filepath, rootData);
        }
        /// <summary>
        /// Reads specified JSON file and deserializes as TRoot type. Finds the specified property and adds the given value to property value, then overwrites the specified file by serializing and writing current TRoot data. 
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="value"></param>
        public static void AddValueToJsonFile<TRoot, TValue>(string filepath, Expression<Func<TRoot, IEnumerable<TValue>>> propertyExpression, TValue value) where TRoot : class
        {
            ValidateValueType(typeof(TValue));
            TRoot rootData = GetData<TRoot>(filepath);
            AddData(rootData, propertyExpression, value);
            WriteToFile(filepath, rootData);
        }

        /// <summary>
        /// Reads specified JSON file and deserializes as TRoot type. Finds the specified property and adds the given values to property value, then overwrites the specified file by serializing and writing current TRoot data.
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="values"></param>
        public static void AddValueToJsonFile<TRoot, TValue>(string filepath, Expression<Func<TRoot, IEnumerable<TValue>>> propertyExpression, IEnumerable<TValue> values) where TRoot : class
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
        private static void AddData<TRoot, TValue>(TRoot rootData, Expression<Func<TRoot, TValue>> propertyExpression, TValue value) where TRoot : class
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
        private static void AddData<TRoot, TValue>(TRoot rootData, Expression<Func<TRoot, IEnumerable<TValue>>> propertyExpression, IEnumerable<TValue> values) where TRoot : class
        {
            HandlePropertyValuesByExpression(propertyExpression, values, rootData);
        }
        /// <summary>
        /// Finds the property of rootData with specified property name and adds value. If property not found, throws exception
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="rootData"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="value"></param>
        private static void AddData<TRoot, TValue>(TRoot rootData, Expression<Func<TRoot, IEnumerable<TValue>>> propertyExpression, TValue value) where TRoot : class
        {
            HandlePropertyValuesByExpression(propertyExpression, value, rootData);
        }

        /// <summary>
        /// Sets given value to the specified property captured by expression
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <param name="rootData"></param>
        /// <exception cref="Exception"></exception>
        private static void HandlePropertyValueByExpression<TRoot, TValue>(Expression<Func<TRoot, TValue>> expression, TValue value, TRoot rootData)
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
        /// Adds values to specified property captured by expression
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression"></param>
        /// <param name="values"></param>
        /// <param name="rootData"></param>
        /// <exception cref="Exception"></exception>
        private static void HandlePropertyValuesByExpression<TRoot, TValue>(Expression<Func<TRoot, IEnumerable<TValue>>> expression, IEnumerable<TValue> values, TRoot rootData)
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
                PropertyInfo searchedProperty = null;
                if (parentValue != null)
                    searchedProperty = parentValue.GetType().GetProperty(property.Name);


                if (searchedProperty != null)
                {
                    var listEnumerable = searchedProperty.GetValue(parentValue) as IEnumerable<TValue>;
                    List<TValue> list = null;

                    if (listEnumerable is null)
                        list = new List<TValue>();
                    else
                        list = listEnumerable.ToList();

                    list.AddRange(values);
                    searchedProperty.SetValue(parentValue, list);
                    parentProperty.SetValue(rootData, parentValue);
                }
                else
                {
                    var listEnumerable = parentValue as IEnumerable<TValue>;
                    List<TValue> list = null;

                    if (listEnumerable is null)
                        list = new List<TValue>();
                    else
                        list = listEnumerable.ToList();

                    list.AddRange(values);
                    parentProperty.SetValue(rootData, list);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Adds value to specified property captured by expression
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <param name="rootData"></param>
        /// <exception cref="Exception"></exception>
        private static void HandlePropertyValuesByExpression<TRoot, TValue>(Expression<Func<TRoot, IEnumerable<TValue>>> expression, TValue value, TRoot rootData)
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

                if (searchedProperty != null)
                {
                    var listEnumerable = searchedProperty.GetValue(parentValue) as IEnumerable<TValue>;
                    List<TValue> list = null;

                    if (listEnumerable is null)
                        list = new List<TValue>();
                    else
                        list = listEnumerable.ToList();

                    list.Add(value);
                    searchedProperty.SetValue(parentValue, list);
                    parentProperty.SetValue(rootData, parentValue);
                }
                else
                {
                    var listEnumerable = parentValue as IEnumerable<TValue>;
                    List<TValue> list = null;

                    if (listEnumerable is null)
                        list = new List<TValue>();
                    else
                        list = listEnumerable.ToList();

                    list.Add(value);
                    parentProperty.SetValue(rootData, list);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Ensures that the given type is implements IConvertible interface or it is a custom written class
        /// </summary>
        /// <param name="valueType"></param>
        /// <exception cref="Exception"></exception>
        private static void ValidateValueType(Type valueType)
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
        /// Gets data as the specified type from JSON file
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <param name="filepath"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        public static TRoot GetData<TRoot>(string filepath) where TRoot : class
        {
            string json = ReadFromFile(filepath);
            TRoot data = null;

            try
            {
                data = JsonSerializer.Deserialize<TRoot>(json);
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
        /// <typeparam name="TRoot"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="data"></param>
        public static void WriteToFile<TRoot>(string filepath, TRoot data) where TRoot : class
        {
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filepath, json);
        }
    }
}