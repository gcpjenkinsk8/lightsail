using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Web;
using Dapper;
using DARReferenceData.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using Spire.Xls;

namespace DARReferenceData.DatabaseHandlers
{
    public class Price : RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        public static int GetCount()
        {
            string sql = $@"
                           SELECT count(*)
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Exchange";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                return connection.Query<int>(sql).FirstOrDefault();
            }
        }



       
     
        public Price()
        {
        }

        public Price(string exchange)
        {
            if (!string.IsNullOrWhiteSpace(exchange))
                Get(exchange);
        }

        
        public PriceViewModel GetLastHourlyPrice(string darAssetID)
        {

            string sql = $@"
                           select dar_identifier as  DARAssetID,ticker as Ticker,last(t.usd_price,from_unixtime(effective_time)) as lastPrice
                           from {DARApplicationInfo.CalcPriceDatabase}.vHourly_price t
                           where methodology = 8
                             and dar_identifier = '{darAssetID}'
                             and from_unixtime(effective_time)  > DATE_ADD(now(), INTERVAL -120 MINUTE) 
                             and from_unixtime(effective_time) <= now() 
                             group by  dar_identifier,ticker  

                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                var l = connection.Query<PriceViewModel>(sql).ToList();

                if(l.Any())
                {
                    return l[0];
                }
      
            }

            return null;
      
        }

        public IEnumerable<PriceInputViewModel> GetPriceInput(string darTicker)
        {

            string sql = $@"
                    select name, Pair, Ticker,AVG(USDPrice) as AvgUSDPrice,COUNT(*) as TradeCount, sum(USDPrice* USDSize) as USDVolume
                    from daxanddex.Pricing_engine_input_trades peit 
                    join refmaster_public.exchange e on peit.ExchangeId =e.legacyID 
                    where ticker in ('{darTicker}')
                    and TSTradeDate > DATE_ADD(now(), interval -1 day ) 
                    group by name, Pair , Ticker
                    order by tradeCount

                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                var l = connection.Query<PriceInputViewModel>(sql).ToList();

                return l;

            }


        }

        public IEnumerable<PrincipalMarketPriceDar> GetPrincipalMarketHourly(string[] assetIdentifiers, string methodology, long startSeconds, long endSeconds, string currency, string callerID, bool excludeHoldover)
        {
            string identifiers = (new Asset()).GetDARIdentifierPrice(assetIdentifiers, callerID);

            string sql = $@"
                            select 
                               ticker Ticker
                              ,methodology  Methodology
                              ,windowStart  WindowStart
                              ,windowEnd WindowEnd
                              ,price UsdPrice
                              ,volume PriceVolume
                              ,periodExchangeVolume PrincipalMarketVolume
                              ,effectiveTime EffectiveTime
                              ,priceID  PriceId
                              ,darID  DarIdentifier
                              ,exchangeName  DarExchangeName 
                              ,FORMAT(pricingTier,0) PricingTier
                              ,assetName  AssetName
                              ,Currency
                              ,darExchangeID DarExchangeID
                            from {DARApplicationInfo.CalcPriceDatabase}.v1hPrincipalMarketPrice
                            where darID in (ASSET_LIST)
                              and effectiveTime >= {startSeconds}
                                and effectiveTime < {endSeconds}
                                and methodology = '{methodology}'
                            

                            ";

            sql = sql.Replace("ASSET_LIST", identifiers);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                var l = connection.Query<PrincipalMarketPriceDar>(sql).ToList();

                return l;

            }


        }


        public IEnumerable<MarketCapViewModel> GetHourlyMarketCap(string[] assetIdentifiers, string windowStart, string windowEnd, string clientId)
        {
            
            Client c = new Client();
            var hasAccess = c.HasAccessToProduct(clientId, "MarketCap");
            
            if (hasAccess)
            {
                List<MarketCapViewModel> result = new List<MarketCapViewModel>();

                string identifiers = (new Asset()).GetDARIdentifierPrice(assetIdentifiers, clientId);

                // Remove single quotes and split by comma
                string[] values = identifiers.Replace("'", "").Split(',');

                // Convert the array into a collection (e.g., List)
                List<string> assetIdentifiersList = new List<string>(values);

                // Check if client has permissions to the searched assets
                List<string> permissionedAssetIdentifiers = new List<string>();
                var hasFullAccess = c.HasFullAccess(clientId, "MarketCap");
                if (hasFullAccess)
                {
                    permissionedAssetIdentifiers = values.ToList();
                }
                else
                {
                    foreach (string asset in assetIdentifiersList)
                    {
                        if (c.HasAccessToAsset(clientId, "MarketCap", asset))
                        {
                            permissionedAssetIdentifiers.Add(asset);
                        }
                    }
                }

                if (permissionedAssetIdentifiers.Count != 0)
                {
                    // Convert the list to a comma-separated string with single quotes around each element
                    string assetIdentifiersString = string.Join(",", permissionedAssetIdentifiers.Select(identifier => $"'{identifier}'"));

                    // Wrap the formatted string in parentheses
                    string formattedAssetIdentifiers = $"({assetIdentifiersString})";

                    string sql = $@"
                                  select 
                                          priceIdentifier
                                          ,darAssetID
                                          ,darAssetTicker
                                          ,marketCap
                                          ,REPLACE(DATE_FORMAT(effectiveTime, '%Y-%m-%dT%TZ'),'Z','+00:00') as effectiveTime
                                  from {DARApplicationInfo.CalcPriceDatabase}.vMarketCap
                                  where (darAssetID in {formattedAssetIdentifiers}
                                    or darAssetTicker in {formattedAssetIdentifiers})
                                    and effectiveTime >= '{windowStart}'
                                    and effectiveTime <= '{windowEnd}'
                                    ORDER BY effectiveTime;
                                ";

                    using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
                    {
                        result = connection.Query<MarketCapViewModel>(sql).ToList();
                    }

                    List<string> upperCaseIdentifiers = assetIdentifiers.Select(identifier => identifier.ToUpper()).ToList();

                    List<string> missingIdentifiers = upperCaseIdentifiers
                    .Except(result.Select(item => (string)item.darAssetTicker))
                    .Except(result.Select(item => (string)item.darAssetID))
                    .ToList();

                    string missingIdentifiersString = string.Join(",", missingIdentifiers.Select(identifier => $"'{identifier}'"));

                    if (missingIdentifiers.Count() > 0)
                    {
                        var i = new MarketCapViewModel()
                        {
                            error = $"We were unable to return Market Cap information for the following assetIdentifier(s): {missingIdentifiersString}. This can occur if the data is unavailable or if you are not permissioned for this asset. Please contact support@digitalassetresearch.com for assistance."
                        };
                        result.Add(i);
                    }
                    return result;
                }
                else
                {
                    throw new Exception($"Client does not have access to the searched assets. Please contact DAR support team at support@digitalassetresearch.com.");
                }

            }
            else
            {
                throw new Exception($"You have not subscribed to Market Cap data. Please contact DAR support team at support@digitalassetresearch.com.");
            }


        }




        public override long Add(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DARViewModel> Get()
        {
            throw new NotImplementedException();
        }

        public override DARViewModel Get(string key)
        {
            throw new NotImplementedException();
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

       
        public override bool IdExists(string nextId)
        {
            throw new NotImplementedException();
        }

        public override string GetNextId()
        {
            throw new NotImplementedException();
        }

        
    }
}