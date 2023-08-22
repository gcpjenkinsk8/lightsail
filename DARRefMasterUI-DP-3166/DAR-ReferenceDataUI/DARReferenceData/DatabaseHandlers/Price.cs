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


        public IEnumerable<MarketCapViewModel> GetHourlyMarketCap(string[] assetIdentifiers, string quoteCurrency, string windowStart, string windowEnd, string callerID)
        {
            string identifiers = (new Asset()).GetDARIdentifierPrice(assetIdentifiers, callerID);

            string sql = $@"
                              select 
                                      lower(mc.DARTicker) as DARTicker
                                      ,t.darAssetID as DARAssetID
                                      ,AssetName as AssetName
                                      ,HourlyPrice as HourlyPrice
                                      ,MarketCap as MarketCap
                                      ,PriceID as PriceID
                                      ,OutstandingSupply as OutstandingSupply
                                      ,OutstandingSupplyLoadTime as OutstandingSupplyLoadTime
                                      ,REPLACE(DATE_FORMAT(LoadTime, '%Y-%m-%dT%TZ'),'Z','+00:00') as LoadTime
                                      ,'USD' as QuoteCurrency
                              from {DARApplicationInfo.CalcPriceDatabase}.1hMarketCap mc
                              inner join {DARApplicationInfo.SingleStoreCatalogPublic}.token2 t on mc.darTicker = t.darTicker
                              where t.darAssetID in (ASSET_LIST)
                                and mc.darTicker in ('BTC', 'ETH', 'DOGE')
                                and loadTime > '{windowStart}'
                                and loadTime <= '{windowEnd}'

                            ";

            sql = sql.Replace("ASSET_LIST", identifiers);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                var l = connection.Query<MarketCapViewModel>(sql).ToList();

                return l;

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