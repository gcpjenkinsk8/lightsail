using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Confluent.Kafka;
using DARReferenceData;
using DARReferenceData.DatabaseHandlers;
using DARReferenceData.DatabaseHandlersTest;
using DARReferenceData.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Text.Json;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DARRefDataTest
{
    public class AmazonUploader
    {
        public bool sendMyFileToS3(System.IO.Stream localFilePath, string bucketName, string subDirectoryInBucket, string fileNameInS3)
        {
            IAmazonS3 client = new AmazonS3Client(RegionEndpoint.APSouth1);
            TransferUtility utility = new TransferUtility(client);
            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

            if (subDirectoryInBucket == "" || subDirectoryInBucket == null)
            {
                request.BucketName = bucketName; //no subdirectory just bucket name
            }
            else
            {   // subdirectory and bucket name
                request.BucketName = bucketName + @"/" + subDirectoryInBucket;
            }
            request.Key = fileNameInS3; //file name up in S3
            request.InputStream = localFilePath;
            utility.Upload(request); //commensing the transfer

            return true; //indicate that the file was sent
        }
    }

    internal class Program
    {
        public static string GetConnectionString()
        {
            return System.IO.File.ReadAllText(@"C:\store\data.txt");
        }

        public static string GetConnectionStringSingleStorePublic()
        {
            return System.IO.File.ReadAllText(@"C:\store\singlestorepublic.txt");
        }

        public static string GetConnectionStringSingleStorePublicDev()
        {
            return System.IO.File.ReadAllText(@"C:\store\singlestorepublic_dev.txt");
        }

        private static void Main(string[] args)
        {
            DARApplicationInfo.SingleStoreInternalDB = GetConnectionStringSingleStorePublicDev();
            //DARApplicationInfo.SingleStoreInternalDB = GetConnectionStringSingleStorePublicDev();
            DARApplicationInfo.CurrentDB = "ReferenceCore-Dev";
            DARApplicationInfo.SingleStorePublicDB = GetConnectionStringSingleStorePublicDev();
            //DARApplicationInfo.SingleStorePublicDB = GetConnectionStringSingleStorePublicDev();
            DARApplicationInfo.SingleStoreCatalogInternal = "refmaster_internal";
            DARApplicationInfo.SingleStoreCatalogPublic = "refmaster_public_DEV";
            DARApplicationInfo.CalcPriceDatabase = "calcprice";
            DARApplicationInfo.BlockChainDatabase = "blockchain";
            DARApplicationInfo.ClientReportingDatabase = "clientReporting_DEV";
            //TEST_GetPrincipalMarketHourly();

            //TEST_TestCalcPrice();
            //TEST_Caching();
            //TEST_LookupAssetIdFromDB();
            //TEST_GetDARIdentifierList();
            //TEST_HasAccess();
            // TEST_DerivativesRisk();
            // PopulateExchnagePair();
            //TEST_CallRestApi();
            // TEST_ClientAccess();
            //TEST_Kafka();
            //TEST_Caching();

            //TEST_CreateHistoryPriceFile();
            //TEST_TestScript();
            //TEST_AssetUpdate();
            //TEST_GetNextAssetID();
            //RUN_RegrassionTest();

            //TEST_SynchFromSingleStore();
            //TEST_GetDARIdentifierList();
            //TEST_GetNextAssetID();
            //TEST_AssetUpdate();
            //TEST_SynchFromSingleStore();

            //LOAD_SeringList_From_Legacy();
            //TEST_ExchangePair();

            //LOAD_CreateBBGServingList();
            //LOAD_SeringList_From_Legacy();
            //LOAD_ServingList();
            //TEST_GetCirculatingSupply();
            TEST_MarketCap();
            //TEST_ClientHasFullAccess();
            //TEST_GetClientAssets();
            //TEST_GetServingList();

            //TEST_LiquidityPoolPrice();
            //TEST_CallBitcoinNode();
            //TEST_get_btc_balockchain_txn();
            //TEST_get_btc_balockchain_pos();

            //TEST_HighLow();

            //TEST_ClientProductAccess();
            //TEST_ClientProductAssetAccess();
            //TEST_VerifiableVolume();
            return;

        }

        private static void TEST_VerifiableVolume()
        {
            Client c = new Client();
            //string clientId = "7nglrdncs3t7dg9ggql9i7a12g";
            //string clientId = "2mnd0b4gasuj6p08s35kiaat14";
            string clientId = "1aeecu0i4qm6jq7nmuss4p2uhl";

            if (true)
            {
                DARReferenceData.DatabaseHandlers.VerifiableVolume v = new DARReferenceData.DatabaseHandlers.VerifiableVolume();
                var result = v.GetVerifiableVolume(new string[] { "waqar" }, "2023-01-30 01:10:32", "2023-01-31 02:10:32", clientId);
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
            else
            {
                Console.WriteLine("Access denied");
            }
        }
        private static void TEST_ClientProductAccess()
        {
            Client c = new Client();
            string clientId = "7nglrdncs3t7dg9ggql9i7a12g";
            string product = "15sPrice";

            for (int i = 0; i < 10; i++)
            {
                var result = c.HasAccessToProduct(clientId, product );
                Console.WriteLine($"Client id:{clientId} product:{product} Access:{result}");

            }

        }
        private static void TEST_ClientProductAssetAccess()
        {
            Client c = new Client();
            string clientId = "46mpjh6qqt3c4o0cigk29j46pp";
            //string clientId = "1aeecu0i4qm6jq7nmuss4p2uhl";
            string product = "15sPrice";
            string asset = "DAMFI9C";

            for (int i = 0; i < 10; i++)
            {
                var result = c.HasAccessToAsset(clientId, product,asset);
                Console.WriteLine($"Client id:{clientId} product:{product} asset:{asset} Access:{result}");

            }

        }


        private static void TEST_HighLow()
        {
            Client c = new Client();
            string clientId = "7nglrdncs3t7dg9ggql9i7a12g";
            string[] assetIdentifiers = { "DASK8KY", "BTC" };
            string windowStart = "2023-07-10 22:29:00";
            string windowEnd = "2023-07-14 12:00:00";
            //Implement a mehtod like HasAccessToHighLow(clientid)

            if (true)
            {
                OHLCV o = new OHLCV();
                var r = o.GetHighLowPrice(clientId, assetIdentifiers, windowStart, windowEnd);
                var json = JsonConvert.SerializeObject(r);
                Console.WriteLine(json);
            }
            else
            {
                Console.WriteLine("Access denied");
            }
        }


        private static void TEST_MarketCap()
        {
            Client c = new Client();
            //string clientId = "7nglrdncs3t7dg9ggql9i7a12g";
            //string clientId = "2mnd0b4gasuj6p08s35kiaat14";
            string clientId = "1aeecu0i4qm6jq7nmuss4p2uhl";

            if (true)
            {
                DARReferenceData.DatabaseHandlers.Price p = new DARReferenceData.DatabaseHandlers.Price();
                var result = p.GetHourlyMarketCap(new string[] { "waqar" }, "2023-01-30 01:10:32", "2023-01-31 02:10:32", clientId);
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
            else
            {
                Console.WriteLine("Access denied");
            }
        }

        private static void TEST_get_btc_balockchain_pos()
        {
            BlockChain bc = new BlockChain();
            string client_id = "7nglrdncs3t7dg9ggql9i7a12g";
            // 
           var r = bc.get_btc_balockchain_pos(new string[] { "btc" },new string[] { "bc1qjasf9z3h7w3jspkhtgatgpyvvzgpa2wwd2lr0eh5tx44reyn2k7sfc27a4", "159X3mWmzxb1uYwG4ZyFGEHmMR98wdJBqM" },"Bitcoin", "2023-05-15T16:25:00", client_id, 50);
            //var r = bc.get_btc_balockchain_pos(new string[] { "*" }, new string[] { "3B3UWYuJiqTr8CGMhjwb9s6PTs7fvVfJG3", "abc" }, "Bitcoin", "2022-06-26 07:23:17.000000", client_id, 50);
            //var r = bc.get_btc_balockchain_pos(new string[] { "XYZ","ABC" }, new string[] { "3B3UWYuJiqTr8CGMhjwb9s6PTs7fvVfJG3", "abc" }, "Bitcoin", "2022-06-26 07:23:17.000000", client_id, 50);

            var jsonWrapper = new
            {
                positions = r
            };

            //var json = JsonSerializer.Serialize<ClientAccessViewModel>(cv);
            var json = JsonConvert.SerializeObject(jsonWrapper);
            Console.WriteLine(json);
        }
        private static void TEST_get_btc_balockchain_txn()
        {
            // &time=2023-05-02T16%253A25%253A00


            BlockChain bc = new BlockChain();
            string client_id = "7nglrdncs3t7dg9ggql9i7a12g";
             var r = bc.get_btc_balockchain_txn(new string[] { "*" }, new string[] { "bc1qjasf9z3h7w3jspkhtgatgpyvvzgpa2wwd2lr0eh5tx44reyn2k7sfc27a4" } , "Bitcoin", "2023-04-02 16:25:00.000000", "2023-05-20 16:25:00.000000", client_id, 50);
            // var r = bc.get_btc_balockchain_txn(new string[] { "BTC", "UNKNOWN" }, new string[] { "121WQkYeoWuPZB1JC6mxtPeRK4y58XoaSy", "3KAjthyhgASmRMdejnL5H9PwsEjUXtHb57" }, "Bitcoin", "2022-01-14 05:05:44.000000", "2023-01-15 05:07:44.000000", client_id, 10);
            //var r = bc.get_btc_balockchain_txn(new string[] { "*" }, new string[] { "121WQkYeoWuPZB1JC6mxtPeRK4y58XoaSy", "3KAjthyhgASmRMdejnL5H9PwsEjUXtHb57" }, "Bitcoin", "2022-01-14 05:05:44.000000", "2023-01-15 05:07:44.000000", client_id, 10);

            var jsonWrapper = new
            {
                blockchaintransaction = r
            };

            //var json = JsonSerializer.Serialize<ClientAccessViewModel>(cv);
            var json = JsonConvert.SerializeObject(jsonWrapper);
            Console.WriteLine(json);

        }
        public static async void get_best_block_hash()
        {
            try
            {
                Console.WriteLine("Hello");
                HttpClient client = new HttpClient();

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://52.206.34.150:8332/");

                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("btcrpc:btc@2023")));
                //Works request.Content = new StringContent("{\"jsonrpc\": \"1.0\", \"id\": \"curltest\", \"method\": \"getblock\", \"params\": [\"000000000000000000016bfb93351e1c761800edddf63588d2cf9a95fde492d2\"]}");
                // request.Content = new StringContent("{\"jsonrpc\": \"1.0\", \"id\": \"curltest\", \"method\": \"getaddressinfo\", \"params\": [\"3B3UWYuJiqTr8CGMhjwb9s6PTs7fvVfJG3\"]}");
                // Fails request.Content = new StringContent("{\"jsonrpc\": \"1.0\", \"id\": \"curltest\", \"method\": \"gettransaction\", \"params\": [\"97b1dd110c3f7e3ec7a8810ff2490e052d5ddf86ea78565fa4c96f37a07c7ffa\"]}");
                request.Content = new StringContent("{\"jsonrpc\": \"1.0\", \"id\": \"curltest\", \"method\": \"getreceivedbyaddress\", \"params\": [\"3B3UWYuJiqTr8CGMhjwb9s6PTs7fvVfJG3\"]}");
                

                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/html; charset=utf-8");

                HttpResponseMessage response = client.Send(request);
                response.EnsureSuccessStatusCode();
                var responseBody = response.Content.ReadAsStream();
                StreamReader reader = new StreamReader(responseBody);
                string text = reader.ReadToEnd();
                Console.WriteLine("INFO: ******************");
                Console.WriteLine(responseBody);
                Console.WriteLine("INFO: ******************");

            }
            
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:******************");
                Console.WriteLine(ex.Message);
                Console.WriteLine("ERROR:******************");
            }
        }
        private static void TEST_CallBitcoinNode()
        {
            CallBitcoinNode o = new CallBitcoinNode();
            Task t = new Task(get_best_block_hash);
            t.Start();
            t.Wait();
            Console.WriteLine("Task completed");
        }

        private static void TEST_GetPrincipalMarketHourly()
        {
            DARReferenceData.DatabaseHandlers.Price price = new DARReferenceData.DatabaseHandlers.Price();
            string[] assetIdentifiers = { "AUDIO" };
            var result = price.GetPrincipalMarketHourly(assetIdentifiers, "DARHourlyPrincipalMarketPrice", 1673220600, 1673222400, "USD", "108.14.255.123", false);
        }

        private static void TEST_TestCalcPrice()
        {
            DerivativesRisk dr = new DerivativesRisk();
            //var prices_obsolete = dr.GetPriceData("2023-01-04 15:59:00", "2023-01-04 16:00:00", "BTC-31MAR23-18000-C-DERIBIT", "DE0VEZY");
            //var prices = dr.GetPriceData("2023-01-04 15:59:00", "2023-01-04 16:00:00", "BTC-31MAR23-18000-C-DERIBIT", "DE0VEZY");
            DARApplicationInfo.SingleStoreInternalDB = GetConnectionStringSingleStorePublic();
            var prices = dr.GetPriceData("2023-01-05 23:39:00", "2023-01-05 23:40:00", "DC9OCSEV", "DE0VEZY");
            var prices_obsolete = dr.GetPriceData_obsolete("2023-01-05 23:39:00", "2023-01-05 23:40:00", "DC9OCSEV", "DE0VEZY");
            // var prices1 = dr.GetPriceData("", "", "DCT3KMPF", "DE0VEZY");

            Console.WriteLine("DEV");
            Console.WriteLine(JsonConvert.SerializeObject(prices));
            Console.WriteLine("PROD");
            Console.WriteLine(JsonConvert.SerializeObject(prices_obsolete));
            

            // DARReferenceData.DatabaseHandlers.Price p = new DARReferenceData.DatabaseHandlers.Price();
            // var result = p.GetLastHourlyPrice("DAMFI9C");

        }

        private static void TEST_LookupAssetIdFromDB()
        {
            Asset a = new Asset();
            var result = a.LookupAssetIdFromDB("DAKRUKA");
        }

        private static void TEST_LiquidityPoolPrice()
        {

            LiquidityPool o = new LiquidityPool();
            var result = o.GetLiquityPoolPrice(new string[] { "DLPWCOKRVUQU", "stMATICStablePool.BALANCER.POLYGON"},"USD", "2022-11-04 05:38:23.000000", "2022-11-05 05:38:23.000000", "DAR-LP-Latest");

            var jsonWrapper = new
            {
                LPTokenPrices = result
            };

            //var json = JsonSerializer.Serialize<ClientAccessViewModel>(cv);
            var json = JsonConvert.SerializeObject(jsonWrapper);
            Console.WriteLine(json);

        }
        private static void TEST_ClientAccess()
        {

            Asset a = new Asset();
            ClientAccessViewModel cv = a.GetClientAccess("Chainlink");


            //var json = JsonSerializer.Serialize<ClientAccessViewModel>(cv);
            var json = JsonConvert.SerializeObject(cv);
            Console.WriteLine(json);

        }


        private static void TEST_CallRestApi()
        {
            string strResponseValue = string.Empty;

            var request = (HttpWebRequest) WebRequest.Create("https://cognito-idp.us-east-1.amazonaws.com/us-east-1_6rSnpbQ4J/.well-known/jwks.json");

            request.Method = "GET";

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();


                //Proecess the resppnse stream... (could be JSON, XML or HTML etc..._

                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            strResponseValue = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //We catch non Http 200 responses here.
                strResponseValue = "{\"errorMessages\":[\"" + ex.Message.ToString() + "\"],\"errors\":{}}";
            }
            finally
            {
                if (response != null)
                {
                    ((IDisposable)response).Dispose();
                }
            }

        }

        public static void PopulateExchnagePair()
        {
            DARApplicationInfo.SingleStoreCatalogInternal = "refmaster_internal";
            DARApplicationInfo.SingleStoreCatalogPublic = "refmaster_public";
            ExchangePairs ep = new ExchangePairs();
            var publicExchangePairs = ep.GetExchangePairPublic();
            DARApplicationInfo.SingleStoreCatalogInternal = "refmaster_internal";
            DARApplicationInfo.SingleStoreCatalogPublic = "refmaster_public";
            //DARApplicationInfo.SingleStoreInternalDB = GetConnectionStringSingleStorePublicDev();
            //DARApplicationInfo.SingleStorePublicDB = GetConnectionStringSingleStorePublicDev();
            int count = 0;
            int total = publicExchangePairs.Count();
            foreach (var pair in publicExchangePairs)
            {
                count++;
                try
                {
                    ((ExchangePairsViewModel)pair).StartTime = DateTime.Now;
                    ((ExchangePairsViewModel)pair).EndTime = DateTime.MaxValue;
                    ep.Add(pair);
                    if (count % 100 == 0)
                    {
                        Console.WriteLine($"Processed {count} of {total}");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }



        }

        private static void TEST_HasAccess()
        {
            // 143.165.0.51
            // 108.14.255.123
            Client c = new Client();


            var hasAccessToCirculatingSupply = c.HasAccessToCirculatingSupply("7nglrdncs3t7dg9ggql9i7a12g");
            if(hasAccessToCirculatingSupply)
            {

            }


            for (int i = 0; i < 10; i++)
            {
                var look_back_days = c.LookBackDays("108.14.255.123");
                look_back_days = c.LookBackDays("5rghesb32ujrriuvps6q2ij7nn");
            }

            



            for (int i = 0; i < 10; i++)
            {
                var has_expired = c.HasExpired("5rghesb32ujrriuvps6q2ij7nn");
                has_expired = c.HasExpired("143.165.0.51");
            }



            var result = c.HasAccessToEvents("5rghesb32ujrriuvps6q2ij7nn");
            result = c.HasAccessToDerivatives("5rghesb32ujrriuvps6q2ij7nn");
            result = c.HasAccessToLiquidityPricing("5rghesb32ujrriuvps6q2ij7nn");
            Console.WriteLine(result);

            ClientAsset ca = new ClientAsset();
            var result1 = ca.HasFullAccess("108.14.255.123");
            result = ca.HasFullAccess("7poop6m2tg9s6hq2f3t5ftcbsd");


            System.IO.File.ReadAllText(@"C:\store\singlestorepublic.txt");
        }
        private static void GENERATE_ExchangePairData()
        {
            System.IO.File.ReadAllText(@"C:\store\singlestorepublic.txt");
        }

        private static void TEST_Kafka()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "nonprod-0-digitalassetresearch-b9b3.aivencloud.com:23893",
                ClientId = Dns.GetHostName(),
                EnableIdempotence = false,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "avnadmin",
                SaslPassword = "AVNS_nj_TjUFCxdE_eNL",
                SslCaLocation = @"C:\DAR\Projects\Code\DARWebSocket\Publishers\ca.pem",
                BatchNumMessages = 3000, // Default
                BatchSize = 1000000, // Default
                LingerMs = 0 // Custom

            };

            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    producer.Produce("dar_log_usage_0", new Message<Null, string> { Value = $"a log message at {DateTime.Now}" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }


        private static void TEST_Caching()
        {

            Asset a = new Asset();

            string[] assetIdentifiers = { "0x25788a1a171ec66da6502f9975a15b609ff54cf6+Polygon" };
            var x = a.GetAllAssetIds(assetIdentifiers);


            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine($"Run {i}");
                string[] assetIdentifiers1 = { "0xf20d962a6c8f70c731bd838a3a388d7d48fa6e15+Avalanche", "btc", "DAI8M0M"};
                var aiNew = a.GetAllAssetIds(assetIdentifiers1);
                Thread.Sleep(5000);
            }






        }
        private static void TEST_DerivativesRisk()
        {
            DerivativesRisk dr = new DerivativesRisk();
            var x = DateTimeOffset.FromUnixTimeSeconds(1657749003);
            Derivatives d = new Derivatives();

            var prices = dr.GetPriceData("", "", "BTC-31MAR23-18000-C-DERIBIT", "DE0VEZY");

            var id = d.GetNextId();

            var data = dr.GetRiskData(1657732927, 1657732929, "SOL-30SEP22-90-C", "DE0VEZY");
        }

        private static void TEST_CreateHistoryPriceFile()
        {
            ServingListFromCurrent_FTSE_DAR_Engine o = new ServingListFromCurrent_FTSE_DAR_Engine();
            o.CreateHistoryPriceFile();
        }

        private static void TEST_TestScript()
        {
            ExchangeTest et = new ExchangeTest();
            ExchangeViewModel e = new ExchangeViewModel();
            Dictionary<string, string> expectedValue = new Dictionary<string, string>();
            Dictionary<string, string> errors = new Dictionary<string, string>();
            expectedValue.Add("ShortName", "TEST123");
            expectedValue.Add("CreateTime", (new DateTime(1973, 6, 16)).ToString("yyyyMMddHHMMss"));
            et.LoadTestData(e);

            e.ShortName = "TEST123";
            e.CreateTime = new DateTime(1973, 6, 16);

            et.ValidateData(e, expectedValue, ref errors);

            if (errors.Any())
            {
            }
        }

        private static void RUN_RegrassionTest()
        {
            AssetTest at = new AssetTest();
            at.RunTest();
        }

        private static void TEST_GetNextAssetID()
        {
            Asset a = new Asset();
            int total = 1000000000;
            for (int i = 0; i < total; i++)
            {
                var x = a.GetNextId();

                if (i % 10000000 == 0)
                {
                    Console.WriteLine($"Processed {i} of {total}");
                }
            }
        }

        private static void TEST_AssetUpdate()
        {
            Asset a = new Asset();

            AssetViewModel vm = new AssetViewModel();
            vm.DARTicker = "XYZ123";
            vm.Name = "XYZ123";
            vm.AssetType = "Token";
            vm.Description = "XYZ123";

            a.Add(vm);

            //string errors;
            //a.LoadFromExcelFileNew(@"c:\delete\AssetUploadTemplate.xlsx",out errors);
        }

        private static void TEST_GetDARIdentifierList()
        {
            Asset a = new Asset();
            string[] assetIdentifiers1 = { "BTC" };
            var resultReference1 = a.GetDARIdentifierReference(assetIdentifiers1, "108.14.255.123");
            var result = a.GetAllAssetIds(assetIdentifiers1);
            var resultPrice1 = a.GetDARIdentifierPrice(assetIdentifiers1, "108.14.255.123");

            string[] assetIdentifiersP = { "btc"};
            var resultP = a.GetDARIdentifierPrice(assetIdentifiersP, "108.14.255.123");


            for (int i = 0; i < 10; i++)
            {
                string[] assetIdentifiers = { "0xf20d962a6c8f70c731bd838a3a388d7d48fa6e15+Avalanche", "btc", "DAI8M0M" };
                var resultPrice = a.GetDARIdentifierPrice(assetIdentifiers, "108.14.255.123");
                Console.WriteLine($"Price filter:{resultPrice}");
                var resultRefer = a.GetDARIdentifierReference(assetIdentifiers, "108.14.255.123");
                Console.WriteLine($"Reference filter:{resultPrice}");
            }
        }

        private static void TEST_SynchFromSingleStore()
        {
            List<AssetRefMasterPublic> notFound = new List<AssetRefMasterPublic>();
            List<AssetViewModel> assetsMissingInToken = new List<AssetViewModel>();
            Dictionary<string, int> tokenAssets = new Dictionary<string, int>();
            Asset asset = new Asset();
            /*
            asset.ReplicateAsset("DAWSKNC");

            Call this to list assets that are missing in ref master public
            var result = asset.GetRefMasterPublicAssets().Cast<AssetRefMasterPublic>().ToList();
            foreach (var resultItem in result)
            {
                if(!tokenAssets.ContainsKey(resultItem.darAssetID))
                {
                    tokenAssets.Add(resultItem.darAssetID,resultItem.legacyID);
                }
            }

            var assets = asset.Get().Cast<AssetViewModel>().ToList();

            foreach(var a in assets)
            {
                if(!tokenAssets.ContainsKey(a.DARAssetID))
                {
                    if (!tokenAssets.ContainsKey(a.LegacyDARAssetId))
                    {
                        assetsMissingInToken.Add(a);
                    }
                }
            }
            using (StreamWriter outputFile = new StreamWriter(@$"c:\temp\AssetsMissingInToken.csv", true))
            {
                foreach (var item in assetsMissingInToken)
                {
                    outputFile.WriteLine($"CALL sp_upsert_asset('{item.DARTicker}','{item.Name}',0,0,3,0,'{item.DARAssetID}');");
                }
            }
            */

            /* run following to find out what assets are missing in ref master
            foreach (var resultItem in result)
            {
                Asset a = new Asset(resultItem.darAssetID);
                if(a.CurrentAsset == null)
                {
                    a = new Asset(resultItem.legacyID.ToString()); // Search by legacy id
                    if (a.CurrentAsset == null)
                    {
                        a = new Asset(resultItem.darTicker); // Search by Ticker
                        if (a.CurrentAsset == null)
                        {
                            a = new Asset(resultItem.name); // Search by Name
                            if(a.CurrentAsset == null)
                            {
                                Console.WriteLine("Failed to find asset. We may need to create this asset in reference master");
                                notFound.Add(resultItem);
                            }
                        }
                    }
                }
            }

            using (StreamWriter outputFile = new StreamWriter(@$"c:\temp\AssetsMissingInRefMaster.csv",true))
            {
                foreach (var item in notFound)
                {
                    outputFile.WriteLine($"{item.name},{item.darTicker},{item.darAssetID},{item.legacyID}");
                }
            }

            */
        }

        private static void TEST_ExchangePair()
        {
            string errors;
            ExchangePair ep = new ExchangePair();

            ExchangePairViewModel vm = new ExchangePairViewModel();
            vm.DARAssetId = "DAMFI9C";
            vm.DARCurrencyId = "DASK8KY";
            vm.DARExchangeId = "DEBLJA9";
            vm.ExchangePairName = "BtcEth";
            vm.ExchangeAssetLongName = "Btc";
            vm.ExchangeAssetNumberId = 1;
            vm.ExchangeAssetShortName = "Eth";
            vm.ExchangeCurrencyStringId = "123";
            ep.UpsertExchangePair(vm, out errors);

            vm = new ExchangePairViewModel();
            vm.DARAssetId = "DAMFI9C";
            vm.DARCurrencyId = "DASK8KY";
            vm.DARExchangeId = "DEBLJA9";

            vm.ExchangePairName = "BTCETH";
            vm.ExchangeAssetLongName = "Btc";
            vm.ExchangeAssetNumberId = 1;
            vm.ExchangeAssetShortName = "Eth";
            vm.ExchangeCurrencyStringId = "123";

            ep.UpsertExchangePair(vm, out errors);
        }

        private static void LOAD_CreateBBGServingList()
        {
            ServingListFromCurrent_FTSE_DAR_Engine o = new ServingListFromCurrent_FTSE_DAR_Engine();
            o.GenerateBBGServingList(@"C:\Delete\bbg_top_assets.csv", "BBG_4PM_File");
        }

        private static void UPLOAD_ServingList()
        {
            /*
            string name = @"c:\Delete\FTSE_Pricing_Engine_17530101_99991231.csv";
            string myBucketName = "cryptoevents"; //your s3 bucket name goes here
            string s3DirectoryName = "";
            string s3FileName = @name;
            bool a;
            AmazonUploader myUploader = new AmazonUploader();
            a = myUploader.sendMyFileToS3(st, myBucketName, s3DirectoryName, s3FileName);
            if (a == true)
            {
                Console.WriteLine("successfully uploaded");
            }
            else
                Console.WriteLine("Error");
            */
        }

        private static void UPLOAD_ServingList_TOS3()
        {
            /*
            string name = @"c:\Delete\FTSE_Pricing_Engine_17530101_99991231.csv";
            string myBucketName = "cryptoevents"; //your s3 bucket name goes here
            string s3DirectoryName = "";
            string s3FileName = @name;
            bool a;
            AmazonUploader myUploader = new AmazonUploader();
            a = myUploader.sendMyFileToS3(st, myBucketName, s3DirectoryName, s3FileName);
            if (a == true)
            {
                Console.WriteLine("successfully uploaded");
            }
            else
                Console.WriteLine("Error");
            */
        }

        private static void LOAD_SeringList_From_Legacy()
        {
            ServingListFromCurrent_FTSE_DAR_Engine o = new ServingListFromCurrent_FTSE_DAR_Engine();
            o.Process();
        }

        private static void LOAD_ServingList()
        {
            CreateServingList o = new CreateServingList();
            o.Load();
        }

        private static void TEST_GetCirculatingSupply()
        {
            DateTime asOfDate = DateTime.Now;
            OutstandingSupply o = new OutstandingSupply();
            var result = o.GetClientCirculatingSupply(asOfDate, "7nglrdncs3t7dg9ggql9i7a12g");
            Console.WriteLine($"Circulating supply count(*): {result.Count()}");
        }

        private static void TEST_ClientHasFullAccess()
        {
            string callerID = "72.80.203.177";
            ClientAsset ca = new ClientAsset();
            var result = ca.HasFullAccess(callerID);
            Console.WriteLine($"Has full price access: {result}");
        }

        private static void TEST_GetClientAssets()
        {
            string callerID = "72.80.203.177";
            ClientAsset ca = new ClientAsset();
            var result = ca.GetAuthorizedAssets(callerID).Cast<ClientAssetsViewModel>().ToList();
            Console.WriteLine($"Total row count for active only: {result.Count}");
        }

        private static void TEST_GetServingList()
        {
            string processName = "PricingMethod1";

            ServingList sl = new ServingList();
            var result = sl.GetServingList(processName).Cast<ServingListSnapshotViewModel>().ToList();
            Console.WriteLine($"Total row count for active only: {result.Count}");

            DateTime start = new DateTime(2022, 1, 5, 22, 12, 0);
            DateTime end = new DateTime(2022, 1, 19, 22, 12, 0);

            result = sl.GetServingList(processName, start, end).Cast<ServingListSnapshotViewModel>().ToList();
            Console.WriteLine($"Total row count for start date and end date: {result.Count}");
        }
    }
}