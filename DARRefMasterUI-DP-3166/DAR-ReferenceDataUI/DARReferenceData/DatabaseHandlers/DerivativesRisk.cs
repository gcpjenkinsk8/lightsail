using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.DatabaseHandlers
{
    public class DerivativesRisk : RefDataHandler
    {
        public override long Add(DARViewModel i)
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

        public IEnumerable<DARViewModel> Get(DateTime asOfDate)
        {
            List<DerivativesViewModel> l = new List<DerivativesViewModel>();

            string sql = $@"
                            SELECT d.ID
                                  , a.DARTicker as UnderlierDARTicker
                                  , UnderlierDARAssetID
                                  , ContractType
                                  , OptionType
                                  , ContractTicker
                                  , DARContractID
                                  , ContractExchange
                                  , ContractExchangeDARID
                                  , case when ExpirationDate >= timestamp(current_date()) then 'active' else 'expired' end as Status
                                  , TradingHours
                                  , MinimumTickSize
                                  , SettlementTime
                                  , SettlementType
                                  , SettlementCurrency
                                  , ExpirationDate
                                  , ContractSize
                                  , InitialMargin
                                  , MaintenanceMargin
                                  , MarkPrice
                                  , DeliveryPrice
                                  , DeliveryMethod
                                  , FeesURL
                                  , PositionLimit
                                  , PositionLimitURL
                                  , BlockTradeMinimum
                                  , LinktoTAndC
                                  , FundingRates
                                  , d.IsActive
                                  , d.CreateUser
                                  , d.LastEditUser
                                  , d.CreateTime
                                  , d.LastEditTime
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Derivatives d
                                inner join  {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on d.UnderlierDARAssetID = a.DARAssetID
                                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<DerivativesViewModel>(sql).ToList();
            }

            return l;
        }


        public IEnumerable<DerivativesRiskApiViewModel> GetRiskData(long windowStart, long windowEnd, string contract, string exchange)
        {
            List<DerivativesRiskApiViewModel> l = new List<DerivativesRiskApiViewModel>();

            string sql = $@"
                            select 
                              ContractTicker
                              ,DARContractID
                              ,UnderlierDARTicker
                              ,UnderlierDARAssetID
                              ,ContractType
                              ,OptionType
                              ,ContractExchange
                              ,ContractExchangeDARID
                              ,Timestamp
                              ,Vega
                              ,Theta
                              ,Rho
                              ,Gamma
                              ,Delta
                              ,OpenInterest
                              FROM {DARApplicationInfo.SingleStoreCatalogPublic}.vDerivativesRiskApi d
                              WHERE AsOfDateUnixTime >= {windowStart}
                                AND AsOfDateUnixTime < {windowEnd}
                                AND (ContractTicker = '{contract}' OR DARContractID = '{contract}' )
                                AND ( ContractExchange = '{exchange}' or ContractExchangeDARID = '{exchange}' )
                                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<DerivativesRiskApiViewModel>(sql).ToList();
            }

            return l;
        }


        public IEnumerable<DerivativesPriceApiViewModel> GetPriceData_obsolete(string windowStart, string windowEnd, string darContractID, string darExchangeID)
        {
            List<DerivativesPriceApiViewModel> l = new List<DerivativesPriceApiViewModel>();

            if (string.IsNullOrWhiteSpace(windowEnd))
            {
                windowEnd = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:00");
            }

            string sql = $@"
                         select o.darExchangeID
                              ,o.darContractID
                              ,o.underlierDARAssetID
                              ,o.currencyTicker
                              ,o.bestBidPrice
                              ,o.bestBidPrice * p.lastPrice as bestBidPriceUSD
                              ,o.bestAskPrice
                              ,o.bestAskPrice * p.lastPrice as bestAskPriceUSD
                              ,o.bestBidSize
                              ,o.bestBidSize * p.lastPrice as bestBidSizeUSD
                              ,o.bestAskSize
                              ,o.bestAskSize * p.lastPrice as bestAskSizeUSD
                              ,t.price
                              ,t.price * p.lastPrice as priceUSD
                              ,t.size
                              ,t.size * p.lastPrice as sizeUSD
                              ,t.Side
                              ,t.markPrice
                              ,t.markPrice * p.lastPrice as markPriceUSD
                              ,t.indexPrice
                              ,t.tSTradeDate as tradeDate
                            from
                               (
                                select  darExchangeID,t.darContractID,underlyingDARAssetID as underlierDARAssetID,currencyTicker,bidPrice as bestBidPrice,askPrice as bestAskPrice,bidSize as bestBidSize, askSize as  bestAskSize
                                from orderbook.derivativesOrder_TS t
                                inner join {DARApplicationInfo.SingleStoreCatalogInternal}.DerivativesContractID c on t.darContractID = c.DARContractID
                                where ( c.ContractTicker = '{darContractID}' or c.DARContractID = '{darContractID}')
                                  and darExchangeID = '{darExchangeID}'
                                  and tStampCollected <= '{windowEnd}'
                                  order by tStampCollected desc
                                  limit 1
                                ) o
                             full outer join (
                                      select underlyingDARAssetID,t.darContractID,darExchangeID,
                                      price,size,Side,currencyTicker,markPrice,indexPrice,tSTradeDate
                                      from daxanddex.derivativesTrade_TS t
                                      inner join {DARApplicationInfo.SingleStoreCatalogInternal}.DerivativesContractID c on t.darContractID = c.DARContractID
                                      where ( c.ContractTicker = '{darContractID}' or c.DARContractID = '{darContractID}')
                                        and darExchangeID = '{darExchangeID}'
                                        and tSTradeDate <= '{windowEnd}'
                                        order by tSTradeDate desc
                                        limit 1
                            ) t on o.darExchangeID = t.darExchangeID and o.darContractID = t.darContractID and o.underlierDARAssetID=t.underlyingDARAssetID
                            left join (
                                  select ticker,last(t.usd_price,from_unixtime(effective_time)) as lastPrice
                                from {DARApplicationInfo.CalcPriceDatabase}.vAll_full_window_price t
                                where methodology = 'DAR'
                                  and ticker in ( 'btc','sol','eth')
                                  and from_unixtime(effective_time)  > DATE_ADD('{windowEnd}', INTERVAL -5 MINUTE) 
                                  and from_unixtime(effective_time) <= '{windowEnd}'
                                  group by  ticker   
                            )  p on p.ticker = o.currencyTicker 

                                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<DerivativesPriceApiViewModel>(sql).ToList();
            }

            return l;
        }



        public IEnumerable<DerivativesPriceApiViewModel> GetPriceData(string windowStart, string windowEnd, string darContractID, string darExchangeID)
        {
            List<DerivativesPriceApiViewModel> l = new List<DerivativesPriceApiViewModel>();

            if(string.IsNullOrWhiteSpace(windowEnd))
            {
                windowEnd = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:00");
            }

         
            string sql = $@"
                        select 
                                darExchangeID
                                ,t.darContractID
                                ,underlyingDARAssetID as underlierDARAssetID
                                ,quoteCCY as currencyTicker
                                ,bestBidPrice as bestBidPrice
                                ,bestBidPriceUSD as bestBidPriceUSD
                                ,bestAskPrice  as bestAskPrice
                                ,bestAskPriceUSD
                                ,bestBidSize
                                ,null as  bestBidSizeUSD
                                ,bestAskSize
                                ,null as bestAskSizeUSD
                                ,lastTradePrice as price
                                ,lastTradePrice as priceUSD
                                ,lastTradeSize as size
                                ,lastTradeSizeUSD as sizeUSD
                                ,lastTradeSide as Side
                                ,markPrice
                                ,markPriceUSD as markPriceUSD
                                ,indexPrice
                                ,lastTradeTimestamp as tradeDate
                        from {DARApplicationInfo.CalcPriceDatabase}.1mDerivPrice t
                        where (darContractID = '{darContractID}' or darTicker = '{darContractID}' )
                            and darExchangeID = '{darExchangeID}'
                            and effectiveTimestamp in (select max(effectiveTimestamp) from {DARApplicationInfo.CalcPriceDatabase}.1mDerivPrice where (darContractID = '{darContractID}' or darTicker = '{darContractID}' ) and  effectiveTimestamp <= '{windowEnd}')
                        order by effectiveTimestamp desc
                                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<DerivativesPriceApiViewModel>(sql).ToList();
            }

            return l;
        }




        public override DARViewModel Get(string key)
        {
            throw new NotImplementedException();
        }

        public override string GetNextId()
        {
            throw new NotImplementedException();
        }

        public override bool IdExists(string nextId)
        {
            throw new NotImplementedException();
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            throw new NotImplementedException();
        }
    }
}
