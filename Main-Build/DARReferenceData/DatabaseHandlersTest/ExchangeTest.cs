using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.DatabaseHandlersTest
{
    public class ExchangeTest : TestHandler
    {



        public override bool AddTest()
        {
            try
            {
                TestLog.Append($"<br>BEGIN: {System.Reflection.MethodBase.GetCurrentMethod().Name}</br>");
                TestCount++;

                int errorCount = 0 ;

                ExchangeViewModel e = new ExchangeViewModel();
                Dictionary<string, string> expectedValue = new Dictionary<string, string>();
                Dictionary<string, string> errors = new Dictionary<string, string>();
                expectedValue.Add("ShortName", ExchangeName);
                expectedValue.Add("CreateTime", (new DateTime(1973, 6, 16)).ToString("yyyyMMddHHMMss"));
                LoadTestData(e,LoadType.ADD);

                e.ShortName = ExchangeName;
                e.CreateTime = new DateTime(1973, 6, 16);
            
                Exchange dh = new Exchange();
                long returnId = dh.Add(e);

                e = (ExchangeViewModel)dh.Get(ExchangeName);

              
          
                ValidateData(e, expectedValue, ref errors, LoadType.ADD);

                if (errors.Any())
                {
                    string[] skip = new string[] { "ID", "DARExchangeID", "CreateUser", "CreateTime", "LastEditUser", "LastEditTime" };
                    foreach(var error in errors)
                    {
                        if(!skip.Contains(error.Key))
                        {
                            LogError($"AddExchange Test: {error.Value} </br>");
                            errorCount++;
                        }
                    }

                }


                if (errorCount > 0)
                {
                    FailCount++;
                    return false;
                }
                else
                {
                    PassCount++;
                    return true;
                }

                }
            catch (Exception ex)
            {
                TestLog.AppendLine($"<br>FAIL: Failed {System.Reflection.MethodBase.GetCurrentMethod().Name} Error:{ex.Message}</br>");
                FailCount++;
                return false;
            }
            finally
            {
                TestLog.Append($"<br>END: {System.Reflection.MethodBase.GetCurrentMethod().Name}<br>");
            }

        }



        public override bool DuplicateTest()
        {
            try
            {
                TestLog.Append($"<br>BEGIN: {System.Reflection.MethodBase.GetCurrentMethod().Name}</br>");
                TestCount++;
                Exchange e = new Exchange();
                ExchangeViewModel exchange = new ExchangeViewModel();
                exchange.ShortName = ExchangeName;
                long returnId = e.Add(exchange);
                if(returnId == 0)
                {
                    PassCount++;
                    return true;
                }
            }
            catch 
            {
                PassCount++;
                return true;
            }
            finally
            {
                TestLog.Append($"<br>END: {System.Reflection.MethodBase.GetCurrentMethod().Name}</br>");
            }
            FailCount++;
            return false;
        }

        public override string RunTest()
        {
            TestLog.Append("ExchangeTest - BEGIN </br>");
            CleanUp();
            // Add asset
            AddTest();
            // Add duplicate asset test
            DuplicateTest();
            // Update test
            UpdateTest();

            // GetCount test
            GetCount();

            //TODO Add Exchange vetting status test

            // Delete test
            DeleteTest();

            TestLog.Append("ExchangeTest - END </br>");
            return PrintTestResult();
        }


        private void GetCount()
        {
            try
            {
                TestLog.Append($"<br>BEGIN: {System.Reflection.MethodBase.GetCurrentMethod().Name}</br>");
                TestCount++;


                var result = Exchange.GetCount();
                if(result == 0)
                {
                    FailCount++;
                }
                else
                {
                    PassCount++;
                }

            }
            catch (Exception ex)
            {
                TestLog.AppendLine($"<br>FAIL: Failed {System.Reflection.MethodBase.GetCurrentMethod().Name} Error:{ex.Message}</br>");
                FailCount++;
               
            }
            finally
            {
                TestLog.Append($"<br>END: {System.Reflection.MethodBase.GetCurrentMethod().Name}<br>");
            }

        }

        public override void CleanUp()
        {
            Exchange e = new Exchange(ExchangeName);
            if (e.CurrentExchange != null)
                e.Delete(e.CurrentExchange);

            e.DeleteReplicatedExchange(ExchangeName);


            e = new Exchange(ExchangeNameV1);
            if (e.CurrentExchange != null)
                e.Delete(e.CurrentExchange);

            e.DeleteReplicatedExchange(ExchangeNameV1);
        }

        public override bool UpdateTest()
        {
            try
            {
                TestLog.Append($"<br>BEGIN: {System.Reflection.MethodBase.GetCurrentMethod().Name}</br>");
                TestCount++;

                int errorCount = 0;

                
                Dictionary<string, string> expectedValue = new Dictionary<string, string>();
                Dictionary<string, string> errors = new Dictionary<string, string>();

                Exchange dh = new Exchange();
                ExchangeViewModel eOriginal = (ExchangeViewModel)dh.Get(ExchangeName);

                if(eOriginal == null)
                {
                    LogError($"{ExchangeName} does not exist. Failed to run update test");
                    FailCount++;
                    return false;
                    
                }

                ExchangeViewModel e = (ExchangeViewModel)dh.Get(ExchangeName);

                expectedValue.Add("DARExchangeID", eOriginal.DARExchangeID); 
                expectedValue.Add("ShortName", eOriginal.ShortName);
                LoadTestData(e,LoadType.UPDATE);

                e.DARExchangeID = eOriginal.DARExchangeID;
                e.ShortName = eOriginal.ShortName;
                
                dh.Update(e);

                e = (ExchangeViewModel)dh.Get(eOriginal.DARExchangeID);

                ValidateData(e, expectedValue, ref errors,LoadType.UPDATE);

                if (errors.Any())
                {
                    string[] skip = new string[] { "ID", "CreateUser", "CreateTime", "LastEditUser", "LastEditTime" };
                    foreach (var error in errors)
                    {
                        if (!skip.Contains(error.Key))
                        {
                            LogError($"UpdateExchange Test: {error.Value} </br>");
                            errorCount++;
                        }
                    }

                }


                if (errorCount > 0)
                {
                    FailCount++;
                    return false;
                }
                else
                {
                    PassCount++;
                    return true;
                }

            }
            catch (Exception ex)
            {
                TestLog.AppendLine($"<br>FAIL: Failed {System.Reflection.MethodBase.GetCurrentMethod().Name} Error:{ex.Message}</br>");
                FailCount++;
                return false;
            }
            finally
            {
                TestLog.Append($"<br>END: {System.Reflection.MethodBase.GetCurrentMethod().Name}<br>");
            }

        }

        public override bool DeleteTest()
        {
            try
            {
                TestCount++;
                Exchange dh = new Exchange();
                ExchangeViewModel e = (ExchangeViewModel)dh.Get(ExchangeName);
                dh.Delete(e);
                e = (ExchangeViewModel)dh.Get(ExchangeName);
                if (e == null)
                {
                    PassCount++;
                    return true;
                }
                else
                {
                    FailCount++;
                    return false;
                }

            }
            catch(Exception ex)
            {
                TestLog.Append($"Failed delete test {ex.Message}");
                FailCount++;
                return false;
            }



        }
    }
}
