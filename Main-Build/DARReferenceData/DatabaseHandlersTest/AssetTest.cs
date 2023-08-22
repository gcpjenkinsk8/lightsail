using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.DatabaseHandlersTest
{
    public class AssetTest : TestHandler
    {



        public override bool AddTest()
        {
            try
            {
                TestLog.Append($"<br>BEGIN: {System.Reflection.MethodBase.GetCurrentMethod().Name}</br>");
                TestCount++;

                int errorCount = 0;

                AssetViewModel e = new AssetViewModel();
                Dictionary<string, string> expectedValue = new Dictionary<string, string>();
                Dictionary<string, string> errors = new Dictionary<string, string>();
                expectedValue.Add("Name", AssetName);
                expectedValue.Add("DARTicker", AssetName);
                expectedValue.Add("CreateTime", (new DateTime(1973, 6, 16)).ToString("yyyyMMddHHMMss"));
                LoadTestData(e);

                e.Name = AssetName;
                e.DARTicker = AssetName;
                e.CreateTime = new DateTime(1973, 6, 16);

                Asset dh = new Asset();
                dh.Add(e);

                e = (AssetViewModel)dh.Get(AssetName);



                ValidateData(e, expectedValue, ref errors);

                if (errors.Any())
                {
                    string[] skip = new string[] { "ID", "DARAssetID", "CreateUser", "CreateTime", "LastEditUser", "LastEditTime" };
                    foreach (var error in errors)
                    {
                        if (!skip.Contains(error.Key))
                        {
                            LogError($"AddExchange Test: {error.Value}");
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
                Asset a = new Asset();
                AssetViewModel asset = new AssetViewModel();
                asset.Name = AssetName;
                asset.DARTicker = AssetName;
                asset.Description = AssetName;
                a.Add(asset);
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
            CleanUp();
            // Add asset
            AddTest();
            // Add duplicate asset test
            DuplicateTest();

            return PrintTestResult();
        }

        public override void CleanUp()
        {
            Asset a = new Asset(AssetName);
            if (a.CurrentAsset != null)
                a.Delete(a.CurrentAsset);

            //a.DeleteReplicatedAsset(AssetName);


            a = new Asset(AssetNameV1);
            if (a.CurrentAsset != null)
                a.Delete(a.CurrentAsset);

            //a.DeleteReplicatedAsset(AssetNameV1);
        }

        public override bool UpdateTest()
        {
            throw new NotImplementedException();
        }

        public override bool DeleteTest()
        {
            throw new NotImplementedException();
        }
    }
}
