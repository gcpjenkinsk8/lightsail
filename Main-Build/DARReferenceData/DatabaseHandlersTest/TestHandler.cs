using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.DatabaseHandlersTest
{
    public enum LoadType
    {
        ADD = 0,
        UPDATE = 1,
    }
    public abstract class TestHandler
    {
        

        public const string TEST_STRING = "Test";
        public const string TEST_STRING_UPDATE = "TestUpdate";
        public const bool TEST_BOOLEAN = true;
        public const bool TEST_BOOLEAN_UPDATE = false;
        public const int TEST_INT = 1000;
        public const int TEST_INT_UPDATE = 1001;
        public const double TEST_DOUBLE = 2000;
        public const double TEST_DOUBLE_UPDATE = 2001;
        public const long TEST_LONG = 3000;
        public const long TEST_LONG_UPDATE = 3001;
        public const decimal TEST_DECIMAL = 4000;
        public const decimal TEST_DECIMAL_UPDATE = 4001;
        public static DateTime TEST_DATETIME = new DateTime(2022,01,01);
        public static DateTime TEST_DATETIME_UPDATE = new DateTime(2022, 01, 02);

        public StringBuilder TestLog = new StringBuilder(1000);
        public int TestCount { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }


        public string AssetName { get; set; } 
        public string AssetNameV1 { get; set; }

        public string ExchangeName { get; set; }
        public string ExchangeNameV1 { get; set; }

        public void LogError(string message)
        {
            TestLog.AppendLine(message);
        }


        public object LoadTestData(object o, LoadType loadType = LoadType.ADD)
        {
            foreach (var v in o.GetType().GetProperties()) // or .GetFields()
            {
                if (v.PropertyType == typeof(string))
                {
                    v.SetValue(o, (loadType == LoadType.ADD)? TEST_STRING: TEST_STRING_UPDATE);
                }
                else if (v.PropertyType == typeof(bool) || v.PropertyType == typeof(bool?))
                {
                    v.SetValue(o, (loadType == LoadType.ADD) ? TEST_BOOLEAN: TEST_BOOLEAN_UPDATE);
                }
                else if (v.PropertyType == typeof(int) || v.PropertyType == typeof(int?))
                {
                    v.SetValue(o, (loadType == LoadType.ADD) ? TEST_INT: TEST_INT_UPDATE);
                }
                else if (v.PropertyType == typeof(long) || v.PropertyType == typeof(long?))
                {
                    v.SetValue(o, (loadType == LoadType.ADD) ? TEST_LONG: TEST_LONG_UPDATE);
                }
                else if (v.PropertyType == typeof(double) || v.PropertyType == typeof(double?))
                {
                    v.SetValue(o, (loadType == LoadType.ADD) ? TEST_DOUBLE: TEST_DOUBLE_UPDATE);
                }
                else if (v.PropertyType == typeof(decimal) || v.PropertyType == typeof(decimal?))
                {
                    v.SetValue(o, (loadType == LoadType.ADD) ? TEST_DECIMAL: TEST_DECIMAL_UPDATE);
                }
                else if (v.PropertyType == typeof(DateTime) || v.PropertyType == typeof(DateTime?))
                {
                    v.SetValue(o, (loadType == LoadType.ADD) ? TEST_DATETIME: TEST_DATETIME_UPDATE);
                }
                else
                {
                    throw new Exception($"{v.PropertyType}  not implemented");
                }
            }

            return o;
        }

        public bool ValidateData(object o, Dictionary<string,string> expectedValue, ref Dictionary<string,string> fails, LoadType loadType = LoadType.UPDATE)
        {

            string expectedString = (loadType == LoadType.ADD)? TEST_STRING: TEST_STRING_UPDATE;
            bool expectedBool = (loadType == LoadType.ADD)? TEST_BOOLEAN: TEST_BOOLEAN_UPDATE;
            int expectedInt = (loadType == LoadType.ADD)? TEST_INT: TEST_INT_UPDATE;
            long expectedLong = (loadType == LoadType.ADD)?TEST_LONG: TEST_LONG_UPDATE;
            double expectedDouble = (loadType == LoadType.ADD)? TEST_DOUBLE: TEST_DOUBLE_UPDATE;
            decimal expectedDecimal = (loadType == LoadType.ADD)? TEST_DECIMAL: TEST_DECIMAL_UPDATE;
            DateTime expectedDateTime = (loadType == LoadType.ADD)? TEST_DATETIME: TEST_DATETIME_UPDATE;

            int successCount = 0;
            foreach (var v in o.GetType().GetProperties()) // or .GetFields()
            {
                if (v.PropertyType == typeof(string))
                {
                    if (expectedValue != null && expectedValue.ContainsKey(v.Name))
                    {
                        if (!expectedValue[v.Name].Equals(v.GetValue(o).ToString()))
                        {
                            fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {expectedValue[v.Name]} actual value is {v.GetValue(o).ToString()} ";
                        }
                    }
                    else if (!v.GetValue(o).ToString().Equals(expectedString))
                    {
                        fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {expectedString} actual value is {v.GetValue(o).ToString()} ";
                    }
                    else
                    {
                        successCount++;
                    }
                }
                else if (v.PropertyType == typeof(bool) || v.PropertyType == typeof(bool?))
                {
                    if (expectedValue != null && expectedValue.ContainsKey(v.Name))
                    {
                        if (bool.Parse(expectedValue[v.Name]) != (bool)v.GetValue(o))
                        {
                            fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {bool.Parse(expectedValue[v.Name])} actual value is {(bool)v.GetValue(o)} ";
                        }
                    }
                    else if ((bool)v.GetValue(o) != expectedBool)
                    {
                        fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {expectedBool} actual value is {(bool)v.GetValue(o)} ";
                    }
                    else
                    {
                        successCount++;
                    }
                }
                else if (v.PropertyType == typeof(int) || v.PropertyType == typeof(int?))
                {
                    if (expectedValue != null && expectedValue.ContainsKey(v.Name))
                    {
                        if (int.Parse(expectedValue[v.Name]) != (int)v.GetValue(o))
                        {
                            fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {int.Parse(expectedValue[v.Name])} actual value is {(int)v.GetValue(o)} ";
                        }
                    }
                    else if ((int)v.GetValue(o) != expectedInt)
                    {
                        fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {expectedInt} actual value is {(int)v.GetValue(o)} ";
                    }
                    else
                    {
                        successCount++;
                    }
                }
                else if (v.PropertyType == typeof(long) || v.PropertyType == typeof(long?))
                {
                    if(expectedValue != null &&  expectedValue.ContainsKey(v.Name))
                    {
                        if(long.Parse(expectedValue[v.Name]) != (long)v.GetValue(o))
                        {
                            fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {long.Parse(expectedValue[v.Name])} actual value is {(long)v.GetValue(o)} ";
                        }
                    }
                    else if ((long)v.GetValue(o) != expectedLong)
                    {
                        fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {expectedLong} actual value is {(long)v.GetValue(o)} ";
                    }
                    else
                    {
                        successCount++;
                    }
                   
                }
                else if (v.PropertyType == typeof(double) || v.PropertyType == typeof(double?))
                {
                    if (expectedValue != null && expectedValue.ContainsKey(v.Name))
                    {
                        if (int.Parse(expectedValue[v.Name]) != (double)v.GetValue(o))
                        {
                            fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {double.Parse(expectedValue[v.Name])} actual value is {(double)v.GetValue(o)} ";
                        }
                    }
                    else if ((double)v.GetValue(o) != expectedDouble)
                    {
                        fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {expectedDouble} actual value is {(double)v.GetValue(o)} ";
                    }
                    else
                    {
                        successCount++;
                    }
                }
                else if (v.PropertyType == typeof(decimal) || v.PropertyType == typeof(decimal?))
                {
                    if (expectedValue != null && expectedValue.ContainsKey(v.Name))
                    {
                        if (int.Parse(expectedValue[v.Name]) != (decimal)v.GetValue(o))
                        {
                            fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {decimal.Parse(expectedValue[v.Name])} actual value is {(decimal)v.GetValue(o)} ";
                        }
                    }
                    else if ((decimal)v.GetValue(o) != expectedDecimal)
                    {
                        fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {expectedDecimal} actual value is {(decimal)v.GetValue(o)} ";
                    }
                    else
                    {
                        successCount++;
                    }
                }
                else if (v.PropertyType == typeof(DateTime) || v.PropertyType == typeof(DateTime?))
                {
                    if (expectedValue != null && expectedValue.ContainsKey(v.Name))
                    {
                        if (!expectedValue[v.Name].Equals(((DateTime)v.GetValue(o)).ToString("yyyyMMddHHMMss")))
                        {
                            fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {expectedValue[v.Name]} actual value is {((DateTime)v.GetValue(o)).ToString("yyyyMMddHHMMss")} ";
                        }
                    }
                    else if ((DateTime)v.GetValue(o) != expectedDateTime)
                    {
                        fails[v.Name] = $"Failed to validate {v.Name}. Expected value this {expectedDateTime} actual value is {(DateTime)v.GetValue(o)} ";
                    }
                    else
                    {
                        successCount++;
                    }
                }
                else
                {
                    throw new Exception($"{v.PropertyType}  not implemented");
                }
            }

            if (fails.Any())
                return false;
            
            return true;
        }

        public TestHandler()
        {
            AssetName = $"TEST_ASSET_{DateTime.Today.ToString("yyyyMMdd")}";
            AssetNameV1 = $"{AssetName}.1";
            ExchangeName = $"TEST_EXCHANGE_{DateTime.Today.ToString("yyyyMMdd")}";
            ExchangeNameV1 = $"{ExchangeName}.1";
        }
        public abstract void CleanUp();

        public abstract string RunTest();

        public abstract bool AddTest();

        public abstract bool DuplicateTest();

        public abstract bool UpdateTest();

        public abstract bool DeleteTest();

        public string PrintTestResult()
        {
            TestLog.AppendLine($"<br>Total test count: {TestCount}</br>");
            TestLog.AppendLine($"<br>          Failed: {FailCount}</br>");
            TestLog.AppendLine($"<br>          Passed: {PassCount}</br>");
             return TestLog.ToString();
        }
    }
}
