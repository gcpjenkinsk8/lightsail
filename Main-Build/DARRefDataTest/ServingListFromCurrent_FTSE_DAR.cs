using DARReferenceData;
using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DARRefDataTest
{
    public class Price
    {
        public string effectiveTime { get; set; }
        public string darTicker { get; set; }
        public string darIdentifier { get; set; }
        public string usdPrice { get; set; }
        public string usdVolume { get; set; }
        public string pricingTier { get; set; }
        public string assetName { get; set; }
    }

    public class LegacyExchange1
    {
        public long ID { get; set; }
        public string Exchange { get; set; }
        public string ExchangeProper { get; set; }
        public int NumericCode { get; set; }

        public string RefMasterDescription { get; set; }
    }

    public class LegacyAssets2
    {
        public int LegacyID { get; set; }
        public string dar_identifier { get; set; } 
        public string ShortName { get; set; }
        public string Literal { get; set; }
        
        public int AssetTierCode { get; set; }

        public string RefMasterAssetTierDescription { get; set; }  
    }

    public class ServingListFromCurrent_FTSE_DAR_Engine
    {
        Dictionary<int, string> assetTierCodeToString = new Dictionary<int, string>()
        {
            { 3 ,  "Tier 3" }
            ,{ 2 ,  "Tier 2" }
            ,{1 ,  "Tier 1" }
            ,{9 ,  "Penalized" }
        };

        Dictionary<int, string> exchangeTierCodeToString = new Dictionary<int, string>()
        {
            { 2,   "exchange_status_TBD" }
            ,{ 1,   "Contributing"}
            ,{ 0,   "Not contributing"}
        };

        Dictionary<long, LegacyExchange1> darExchanges = new Dictionary<long, LegacyExchange1>();
        Dictionary<long, LegacyExchange1> ftseExchanges = new Dictionary<long, LegacyExchange1>();

        List<LegacyAssets2> darlegacyAssets =  new List<LegacyAssets2>();
        List<LegacyAssets2> ftselegacyAssets = new List<LegacyAssets2>();

        public void LoadLegacyExchanges()
        {
            string fileName = @"C:\DAR\Projects\Code\ServingList\serving_list_exchanges.csv";
            string[] lines = System.IO.File.ReadAllLines(fileName);
            int lineNumber = 0;
            foreach (string line in lines)
            {
                lineNumber++;
                try
                {


                    string[] lineArray = line.Split(",");

                    if (lineArray.ElementAtOrDefault(0).Equals("ID"))
                        continue;



                    LegacyExchange1 darExchange = new LegacyExchange1();
                    darExchange.ID = int.Parse(lineArray.ElementAtOrDefault(0));
                    darExchange.Exchange = lineArray.ElementAtOrDefault(1);
                    darExchange.ExchangeProper = lineArray.ElementAtOrDefault(2);
                    darExchange.NumericCode = int.Parse(lineArray.ElementAtOrDefault(3));
                    if (exchangeTierCodeToString.ContainsKey(darExchange.NumericCode))
                        darExchange.RefMasterDescription = exchangeTierCodeToString[darExchange.NumericCode];

                    if (!darExchanges.ContainsKey(darExchange.ID))
                        darExchanges.Add(darExchange.ID, darExchange);

                    LegacyExchange1 ftseExchange = new LegacyExchange1();
                    ftseExchange.ID = int.Parse(lineArray.ElementAtOrDefault(0));
                    ftseExchange.Exchange = lineArray.ElementAtOrDefault(1);
                    ftseExchange.ExchangeProper = lineArray.ElementAtOrDefault(2);
                    ftseExchange.NumericCode = int.Parse(lineArray.ElementAtOrDefault(4));
                    if (exchangeTierCodeToString.ContainsKey(ftseExchange.NumericCode))
                        ftseExchange.RefMasterDescription = exchangeTierCodeToString[ftseExchange.NumericCode];

                    if (!ftseExchanges.ContainsKey(ftseExchange.ID))
                        ftseExchanges.Add(darExchange.ID, ftseExchange);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process line {lineNumber} - {fileName}. Error:{ex.Message}");
                }
            }
        }

        public void LoadLegacyAssets(string fileName, ref List<LegacyAssets2> assetList)
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);
            int lineNumber = 0;
            foreach (string line in lines)
            {
                lineNumber++;
                try
                {
                    string[] lineArray = line.Split(",");

                    if (lineArray.ElementAtOrDefault(0).Contains("ID"))
                        continue;
                    //LegacyID,dar_identifier,ShortName,Literal,AssetTier
                    LegacyAssets2 darAsset = new LegacyAssets2();
                    darAsset.LegacyID = int.Parse(lineArray.ElementAtOrDefault(0));
                    darAsset.ShortName = lineArray.ElementAtOrDefault(1);
                    darAsset.Literal = lineArray.ElementAtOrDefault(2);
                    darAsset.dar_identifier = lineArray.ElementAtOrDefault(3);
                    darAsset.AssetTierCode = int.Parse(lineArray.ElementAtOrDefault(4));

                    if (assetTierCodeToString.ContainsKey(darAsset.AssetTierCode))
                    {
                        if (assetTierCodeToString.ContainsKey(darAsset.AssetTierCode))
                            darAsset.RefMasterAssetTierDescription = assetTierCodeToString[darAsset.AssetTierCode];
                    }
                    assetList.Add(darAsset);    

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process line {lineNumber} - {fileName}. Error:{ex.Message}");
                }
            }
        }

        private void WriteToFile(string servingListName,string line)
        {
            using (StreamWriter outputFile = new StreamWriter(@$"c:\temp\{servingListName}.output.csv",true))
            {
                outputFile.WriteLine(line.ToString());
            }
        }


        public string FormatErrorMessage(int rowNumber, string inputline, string exchange, string asset, string quoteAsset, string error)
        {
            return $"{rowNumber},{inputline},{exchange},{asset},{quoteAsset},{error}";
        }

        public void GenerateServingList(string fileName, string servingListName)
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);
            int lineNumber = 0;
            int exchangeId;
            string pair;
            string currencyIdentifier;
            string assetIdentifier=String.Empty;
            string assetTier=String.Empty;
            string exchangeStatus;
            string errorMessage;

            Dictionary<long, LegacyExchange1> exchanges;
            List<LegacyAssets2> assets;
            Listing listingDH = new Listing();

            StringBuilder sbErrors = new StringBuilder();

            Exchange e = new Exchange();
            var refMasterExchanges = e.Get().Cast<ExchangeViewModel>().ToList();

            string outputFileName = @$"c:\temp\{servingListName}.output.csv";

            FileInfo fi = new FileInfo(outputFileName);
            if(fi.Exists)
                fi.Delete();






            if (servingListName.Contains("DAR_Pricing_Engine"))
            {
                exchanges = darExchanges;
                assets = darlegacyAssets;

            }
            else
            {
                exchanges = ftseExchanges;
                assets = ftselegacyAssets;
            }

            int totalLines = lines.Count();

            foreach (string line in lines)
            {
                lineNumber++;
                try
                {
                    string[] lineArray = line.Split(",");

                    if (lineArray.ElementAtOrDefault(0).Contains("Id"))
                        continue;
                    
                    exchangeId = int.Parse(lineArray.ElementAtOrDefault(0));
                    pair = lineArray.ElementAtOrDefault(1);
                    currencyIdentifier = lineArray.ElementAtOrDefault(2);
                    Asset currencyAsset = new Asset(currencyIdentifier);
                    try
                    {
                        assetIdentifier = ServingList.GetAssetFromPair(pair, currencyIdentifier, currencyAsset.CurrentAsset, lineNumber);
                    }
                    catch(Exception ex1)
                    {
                        FormatErrorMessage(lineNumber, line, exchangeId.ToString(), currencyIdentifier, "", ex1.Message);
                    }
                    Asset asset = new Asset(assetIdentifier);


                    if (exchanges.ContainsKey(exchangeId))
                        exchangeStatus = exchanges[exchangeId].RefMasterDescription;
                    else
                    {
                        exchangeStatus = null;
                        errorMessage =  FormatErrorMessage(lineNumber,line,exchangeId.ToString(),"","","Couldn't determine exchange status");
                        Console.WriteLine(errorMessage);
                        sbErrors.AppendLine(errorMessage);
                        continue;
                    }

                    var refExchange = refMasterExchanges.Where(x => x.LegacyId == exchangeId);
                    if (!refExchange.Any() || refExchange.Count() == 0)
                    {
                        errorMessage = FormatErrorMessage(lineNumber,line,exchangeId.ToString(),"","","Couldn't determine exchange from legacyid");
                        Console.WriteLine(errorMessage);
                        sbErrors.AppendLine(errorMessage);
                        continue;
                    }



                    if (currencyAsset.CurrentAsset == null)
                    {
                        var map = listingDH.GetListingById(refExchange.FirstOrDefault().DARExchangeID, currencyIdentifier);
                        if (map != null)
                        {
                            currencyAsset = new Asset(map.DARAssetID);
                        }
                        else
                        {
                            errorMessage = FormatErrorMessage(lineNumber, line, refExchange.FirstOrDefault().ShortName, currencyIdentifier, "", "Can't find quote asset in reference master. Setup asset in ref master if the asset is missing or add an entry in ExchangeAssetMap tab if the asset exists already");
                            Console.WriteLine(errorMessage);
                            sbErrors.AppendLine(errorMessage);
                            continue;
                        }
                        
                    }

                    if (asset.CurrentAsset == null)
                    {
                        //Try to look for asset using legacy asset id
                        var legacyAssetDarId = assets.Where(x => x.ShortName.ToLower().Equals(assetIdentifier.ToLower()) || x.Literal.ToLower().Equals(assetIdentifier.ToLower()));
                        if(legacyAssetDarId != null && legacyAssetDarId.Count() > 0)
                        {
                            asset = new Asset(legacyAssetDarId.FirstOrDefault().dar_identifier);
                            if(asset.CurrentAsset == null)
                            {
                                var map = listingDH.GetListingById(refExchange.FirstOrDefault().DARExchangeID,assetIdentifier);
                                if (map != null)
                                {
                                    asset = new Asset(map.DARAssetID);
                                }
                                errorMessage = FormatErrorMessage(lineNumber, line, refExchange.FirstOrDefault().ShortName, currencyAsset.CurrentAsset.DARAssetID,assetIdentifier, "Can't find  asset in reference master.  Setup asset in ref master if the asset is missing or add an entry in ExchangeAssetMap tab if the asset exists already");
                                Console.WriteLine(errorMessage);
                                sbErrors.AppendLine(errorMessage);
                                continue;
                            }
                        }
                        else
                        {
                            var map = listingDH.GetListingById(refExchange.FirstOrDefault().DARExchangeID,assetIdentifier);
                            if (map != null)
                            {
                                asset = new Asset(map.DARAssetID);
                            }

                            errorMessage = FormatErrorMessage(lineNumber, line, refExchange.FirstOrDefault().ShortName, currencyAsset.CurrentAsset.DARAssetID, assetIdentifier, "Can't find  asset in reference master.  Setup asset in ref master if the asset is missing or add an entry in ExchangeAssetMap tab if the asset exists already");
                            Console.WriteLine(errorMessage);
                            sbErrors.AppendLine(errorMessage);
                            continue;
                        }

              
                    }



                    if (asset.CurrentAsset != null)
                    {
                        var legacyAsset = assets.Where(x => x.dar_identifier.Equals(asset.CurrentAsset.LegacyDARAssetId) || x.LegacyID == asset.CurrentAsset.LegacyId || x.ShortName.ToLower().Equals(asset.CurrentAsset.Name.ToLower()) || x.ShortName.ToLower().Equals(asset.CurrentAsset.DARTicker.ToLower()));
                        if (legacyAsset.Any())
                        {
                            assetTier = legacyAsset.FirstOrDefault().RefMasterAssetTierDescription;
                        }
                        else
                        {
                            assetTier = String.Empty;
                            errorMessage = FormatErrorMessage(lineNumber, line, refExchange.FirstOrDefault().ShortName, currencyAsset.CurrentAsset.DARAssetID, asset.CurrentAsset.DARAssetID, "Couldn't determine asset  status row number ");
                            Console.WriteLine(errorMessage);
                            sbErrors.AppendLine(errorMessage);
                            continue;
                        }
                    }
                    else
                    {
                        errorMessage = FormatErrorMessage(lineNumber, line, refExchange.FirstOrDefault().ShortName, currencyAsset.CurrentAsset.DARAssetID, asset.CurrentAsset.DARAssetID, "Couldn't determine asset  status row number ");
                        Console.WriteLine(errorMessage);
                        sbErrors.AppendLine(errorMessage);
                        continue;
                    }


                    

                    WriteToFile(servingListName, $"{refExchange.FirstOrDefault().ShortName},{pair},{assetIdentifier},{currencyIdentifier},{servingListName},{asset.CurrentAsset.DARAssetID},{currencyAsset.CurrentAsset.DARAssetID},{assetTier},{exchangeStatus}");
                }
                catch (Exception ex)
                {
                    errorMessage = $"Failed to process line {lineNumber} - {fileName}. Error:{ex.Message}";
                    Console.WriteLine(errorMessage);
                    sbErrors.AppendLine(errorMessage);
                }

                if(lineNumber % 100 == 0)
                {
                    Console.WriteLine($"Processed {lineNumber} of {totalLines}");
                }
            }


            using (StreamWriter outputFile = new StreamWriter(@$"c:\temp\{servingListName}.error.csv", false))
            {
                outputFile.WriteLine(sbErrors.ToString());
            }
        }

        public void Process()
        {
            LoadLegacyExchanges();
            LoadLegacyAssets(@"C:\DAR\Projects\Code\ServingList\dar_asset_status.csv", ref darlegacyAssets);
            LoadLegacyAssets(@"C:\DAR\Projects\Code\ServingList\ftse_asset_status.csv", ref ftselegacyAssets);

            GenerateServingList(@"C:\DAR\Projects\Code\ServingList\dar_serving_list.csv", "DAR_Pricing_Engine");
            //GenerateServingList(@"C:\DAR\Projects\Code\ServingList\dar_serving_list_test.csv", "DAR_Pricing_Engine");
            //GenerateServingList(@"C:\DAR\Projects\Code\ServingList\ftse_serving_list.csv", "FTSE_Pricing_Engine");

        }

        public void GenerateBBGServingList(string fileName, string servingListName)
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);
            int lineNumber = 0;
            string assetIdentifier;
            string assetTier = String.Empty;
            string errorMessage;

            StringBuilder sbErrors = new StringBuilder();

            Exchange e = new Exchange();
            var refMasterExchanges = e.Get().Cast<ExchangeViewModel>().ToList();


            int totalLines = lines.Count();

            foreach (string line in lines)
            {
                lineNumber++;
                //Console.WriteLine($"Line {lineNumber}");
                try
                {
                    string[] lineArray = line.Split(",");

                    if (lineArray.ElementAtOrDefault(0).Contains("dar_"))
                        continue;
                    assetIdentifier = lineArray.ElementAtOrDefault(0);
                    Asset asset = new Asset(assetIdentifier);

                                        
                    if (asset.CurrentAsset == null)
                    {
                        errorMessage = $"Can't find  asset {assetIdentifier} in reference master. Line {lineNumber}";
                        Console.WriteLine(errorMessage);
                        sbErrors.AppendLine(errorMessage);
                        continue;
                    }



                    WriteToFile(servingListName, $"BITFRONT,{asset.CurrentAsset.DARTicker}USDT,{asset.CurrentAsset.DARTicker},USDT,{servingListName},{asset.CurrentAsset.DARAssetID},DAHDALR,Tier 1,exchange_status_TBD");
                }
                catch (Exception ex)
                {
                    errorMessage = $"Failed to process line {lineNumber} - {fileName}. Error:{ex.Message}";
                    Console.WriteLine(errorMessage);
                    sbErrors.AppendLine(errorMessage);
                }

                //if (lineNumber % 5 == 0)
                //{
                //    Console.WriteLine($"Processed {lineNumber} of {totalLines}");
                //}
            }


            using (StreamWriter outputFile = new StreamWriter(@$"c:\temp\{servingListName}.error.txt"))
            {
                outputFile.WriteLine(sbErrors.ToString());
            }
        }

        public void UploadServingList()
        {
            string errors;
            ServingList sl = new ServingList();
            sl.LoadDataFromExcelFile(@"c:\temp\FTSE_Serving_List_Upload.xlsx", out errors);
            if(errors != null)
            {
                using (StreamWriter outputFile = new StreamWriter(@$"c:\temp\FTSE_Serving_List_Upload.output.txt", false))
                {
                    outputFile.WriteLine(errors.ToString());
                }
            }
        }


        public void CreateHistoryPriceFile()
        {


            try
            {

                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                keyValuePairs.Add("DAX4BJ6", "0x");
                keyValuePairs.Add("DA9ZRWQ", "1inch");
                keyValuePairs.Add("DA0WV5E", "Aave");
                keyValuePairs.Add("DAPG1UA", "Algorand");
                keyValuePairs.Add("DAWWDPT", "Avalanche");
                keyValuePairs.Add("DAMFI9C", "Bitcoin");
                keyValuePairs.Add("DA22JZ7", "Cardano");
                keyValuePairs.Add("DAPV839", "Chainlink");
                keyValuePairs.Add("DAEF133", "Cosmos");
                keyValuePairs.Add("DA6VBD5", "Decentraland");
                keyValuePairs.Add("DAKUIH5", "Dogecoin");
                keyValuePairs.Add("DAUS07F", "dYdX");
                keyValuePairs.Add("DASK8KY", "Ethereum");
                keyValuePairs.Add("DAXMXEC", "Fantom");
                keyValuePairs.Add("DAK708M", "Filecoin");
                keyValuePairs.Add("DA1YBM0", "Helium");
                keyValuePairs.Add("DAMIAL1", "Litecoin");
                keyValuePairs.Add("DA6GW88", "Maker");
                keyValuePairs.Add("DAXMR75", "PancakeSwap");
                keyValuePairs.Add("DARUEIM", "Polygon");
                keyValuePairs.Add("DAPQYM9", "Serum");
                keyValuePairs.Add("DAXVC4R", "Solana");
                keyValuePairs.Add("DA93HJY", "TerraUSD");
                keyValuePairs.Add("DAHDALR", "Tether");
                keyValuePairs.Add("DAKP21Z", "TRON");
                keyValuePairs.Add("DAJK4YL", "Uniswap");
                keyValuePairs.Add("DANMMKN", "XRP");
                keyValuePairs.Add("DAXNYTJ", "Axie Infinity");
                keyValuePairs.Add("DA6WU6K", "Binance USD");
                keyValuePairs.Add("DARL2F4", "Bitcoin Cash");
                keyValuePairs.Add("DAVRASA", "Convex Finance");
                keyValuePairs.Add("DAQT12J", "Curve DAO Token");
                keyValuePairs.Add("DA14ISM", "Elrond");
                keyValuePairs.Add("DA203GQ", "FTX Token");
                keyValuePairs.Add("DA05GWG", "Injective Protocol");
                keyValuePairs.Add("DAUP87X", "Internet Computer");
                keyValuePairs.Add("DALYJ9J", "Near");
                keyValuePairs.Add("DAYKXMG", "Polkadot");
                keyValuePairs.Add("DA3LCN2", "SHIBA INU");
                keyValuePairs.Add("DA3D6Z0", "Sushi");
                keyValuePairs.Add("DALP93G", "Synthetix");
                keyValuePairs.Add("DAFAYJB", "Terra");
                keyValuePairs.Add("DARMN8J", "The Graph");
                keyValuePairs.Add("DAN2H97", "Sand");
                keyValuePairs.Add("DA7QJTN", "USD Coin");
                keyValuePairs.Add("DAI8MLG", "yearn.finance");
                keyValuePairs.Add("DA2DANE", "Binance Coin");
                keyValuePairs.Add("DA0AANR", "Crypto.com-CRO");
                keyValuePairs.Add("DA3ECBD", "Dai");







                foreach(var keyValue in keyValuePairs)
                {
                    using (StreamWriter outputFile = new StreamWriter(@$"C:\Delete\PriceHistory\{keyValue.Key}_{keyValue.Value}.csv", false))
                    {

                        outputFile.WriteLine("effectiveTime,darTicker,darIdentifier,usdPrice,usdVolume,pricingTier,assetName");


                        string sql = $@"    select effectiveTime,	darTicker,	darIdentifier,	usdPrice,	usdVolume,	pricingTier,	assetName
                                from metadata_DEV.hourlyPriceBackfill
                                where darIdentifier = '{keyValue.Key}'
                        
                            ";
                        using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
                        {
                            var r = connection.Query<Price>(sql);

                            if (r != null && r.Any())
                            {
                                foreach(var item in r)
                                {
                                    outputFile.WriteLine($"{item.effectiveTime},{item.darTicker},{item.darIdentifier},{item.usdPrice},{item.usdVolume},{item.pricingTier},{item.assetName}");
                                }

                            }
                        }
                    }

                    Console.WriteLine($"End {keyValue.Key} {keyValue.Value}");
                }

       
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
       
        }


    }




}
