using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARRefDataTest
{
    enum AssetUploadColumns
    {
        LegacyId = 0
     , ShortName = 1
     , Literal = 2
     , DARIdentifier = 3
     , AssetTierCode = 4
     , CorrectAssetTierCode = 5

    }

    enum ExchangeUploadColumns
    {
        Id=0
        ,Literal=1
        ,DAR=2
        ,FTSE=3
    }
    enum ServingListColumns
    {
        ExchangeId = 0
    , Pair = 1
    , CurrencyIdentifier = 2
    }
    public class LegacyAsset
    {
        public int LegacyId { get; set; }
        public string Shortname { get; set; }
        public string Literal { get; set; }
        public string dar_identifier { get; set; }
        public string AssetTierCode { get; set; }

        public string CorrectDARTier { get; set; }

    }

    public class LegacyExchange
    {
        public long Id { get; set; } 
        public string Literal { get; set; } 
        public string DAR { get; set; } 
        public string FTSE { get; set; }
    }

    public class ServingListItem
    {
        public int LegacyExchangeId { get; set; }
        public string Pair { get; set; }    
        public string CurrencyIdentifier { get; set; }

        public string DARExchangeID { get; set; }    
        public string AssetIdentifier { get; set; }


    }

    public class CreateServingList
    {
 
        Dictionary<string,LegacyAsset> legacyAssets = new Dictionary<string,LegacyAsset>();
        Dictionary<string, LegacyExchange> legacyExchange = new Dictionary<string, LegacyExchange>();
        Dictionary<int,string> exchangeByLegacyId = new Dictionary<int, string>();  

        public void Load()
        {
            ReadLegacyAssets();
            ReadLegacyExchanges();
            LoadExchangeByLegacyId();
            GenerateServingList(@"c:\Delete\ftse_serving_list.xlsx");



        }

        private void LoadExchangeByLegacyId()
        {
            Exchange e = new Exchange();
            var exchanges = e.Get().Cast<ExchangeViewModel>().ToList();
            foreach (var ex in exchanges)
            {
                if(!exchangeByLegacyId.ContainsKey(ex.LegacyId))
                {
                    exchangeByLegacyId.Add(ex.LegacyId, ex.DARExchangeID);
                }
            }

            Console.WriteLine($"exchangeByLegacyId count: {exchangeByLegacyId.Count}");
        }
        private void ReadLegacyExchanges()
        {
            int rowCount = 0;
            try
            {
                string fileName = @"c:\Delete\CorrectExchangeStatus.xlsx";
                Workbook workbook = new Workbook();
                workbook.LoadFromFile(fileName);
                Worksheet sheet = workbook.Worksheets[0];

                LegacyExchange a;

                foreach (var row in sheet.Rows)
                {
                    rowCount++;
                    if (rowCount == 1)
                    {
                        continue;
                    }

                    a = new LegacyExchange();
                    a.Id = int.Parse(row.Columns[(int)ExchangeUploadColumns.Id].Value);
                    a.Literal = row.Columns[(int)ExchangeUploadColumns.Literal].Value;
                    a.DAR = row.Columns[(int)ExchangeUploadColumns.DAR].Value;
                    a.FTSE = row.Columns[(int)ExchangeUploadColumns.FTSE].Value;
                    if (!legacyExchange.ContainsKey(a.Literal))
                        legacyExchange.Add(a.Literal, a);
                    else
                        Console.WriteLine($"{a.Literal} exists already");
                }
                Console.WriteLine($"Legacy Exchage Count: {legacyExchange.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RowCount:{rowCount} ERROR: {ex.Message} ");
            }
        }

        private void ReadLegacyAssets()
        {
            int rowCount = 0;
            try
            {
                string fileName = @"c:\Delete\CorrectAssetTierCodes.xlsx";
                Workbook workbook = new Workbook();
                workbook.LoadFromFile(fileName);
                Worksheet sheet = workbook.Worksheets[0];

                LegacyAsset a;
                
                foreach (var row in sheet.Rows)
                {
                    rowCount++;
                    if(rowCount == 1)
                    {
                        continue;
                    }

                    a = new LegacyAsset();
                    a.LegacyId = int.Parse(row.Columns[(int)AssetUploadColumns.LegacyId].Value);
                    a.Shortname = row.Columns[(int)AssetUploadColumns.ShortName].Value;
                    a.Literal = row.Columns[(int)AssetUploadColumns.Literal].Value;
                    a.dar_identifier = row.Columns[(int) AssetUploadColumns.DARIdentifier].Value;
                    a.AssetTierCode = row.Columns[(int)AssetUploadColumns.AssetTierCode].Value;
                    a.CorrectDARTier = row.Columns[(int)AssetUploadColumns.CorrectAssetTierCode].Value;
                    if (!legacyAssets.ContainsKey(a.dar_identifier))
                        legacyAssets.Add(a.dar_identifier, a);
                    else
                        Console.WriteLine($"{a.dar_identifier} exists already");
                }
                Console.WriteLine($"Legacy Asset Count: {legacyAssets.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RowCount:{rowCount} ERROR: {ex.Message} ");
            }
        }

        private void GenerateServingList(string inputFile)
        {
            int rowCount = 0;
            int errorCount = 0;
            try
            {
                
                Workbook workbook = new Workbook();
                workbook.LoadFromFile(inputFile);
                Worksheet sheet = workbook.Worksheets[0];

                ServingListItem a;

                
                VettingStatus vs = new VettingStatus();
                var exchangeStatusList = vs.Get().Cast<VettingStatusViewModel>().Where(x=>x.StatusType.Equals("Exchange Status")).ToList();
                var assetStatusList = vs.Get().Cast<VettingStatusViewModel>().Where(x => x.StatusType.Equals("Asset Tier")).ToList();



                foreach (var row in sheet.Rows)
                {
                    try
                    {
                        rowCount++;
                        if (rowCount == 1)
                        {
                            continue;
                        }

                        a = new ServingListItem();
                        a.LegacyExchangeId = int.Parse(row.Columns[(int)ServingListColumns.ExchangeId].Value);
                        a.Pair = row.Columns[(int)ServingListColumns.Pair].Value;
                        a.CurrencyIdentifier = row.Columns[(int)ServingListColumns.CurrencyIdentifier].Value;
                        Asset currencyAsset = new Asset(a.CurrencyIdentifier);
                        if(currencyAsset.CurrentAsset != null)
                        {
                            a.AssetIdentifier = ServingList.GetAssetFromPair(a.Pair, a.CurrencyIdentifier,currencyAsset.CurrentAsset,rowCount);

                        }
                        else
                        {
                            throw new Exception($"{a.CurrencyIdentifier} does not exist in reference master. Setup this asset before uploding. Row:{rowCount}");
                        }
                        
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        errorCount++;
                    }

                    if (rowCount % 100 == 0)
                        Console.WriteLine($"Processed: {rowCount} rows");
       
                }
                Console.WriteLine($"Serving list Count: {rowCount}");
                Console.WriteLine($"Failed to upload Count: {errorCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RowCount:{rowCount} ERROR: {ex.Message} ");
            }
        }
    }
}
