using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class LiquidityPool : RefDataHandler
    {

        public LiquidityPool()
        {
        }



        public string GetPoolIdentifiers(string[] darPoolIdentifiers)
        {
            string identifiers = $"'{String.Join("','", darPoolIdentifiers)}'";


            string sql = $@"
                                select distinct DARLiqPoolID 
                                from refmaster_public.vLiquidityPool 
                                where (DARTicker in (IDENTIFIERS) or DARLiqPoolID in (IDENTIFIERS))
                            ";

            sql = sql.Replace("IDENTIFIERS", identifiers);


            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                var l = connection.Query<string>(sql).ToList();

                if (l.Any())
                    return $"'{String.Join("','", l)}'";
            }

            return null;
        }

        public List<LiquidityPoolViewModel> GetLiquityPoolPrice(string[] darPoolIdentifiers
                , string quoteCurrency
                , string windowStart
                , string windowEnd
                , string methodology)

        {
      
            var lAll = new List<LiquidityPoolViewModel>();

            string identifiers = GetPoolIdentifiers(darPoolIdentifiers);


            string sql = $@"
                           select pb.darPoolID
                                    ,vlp.DARTicker as darPoolTicker
                                    ,vlpc.Description as darPoolName
                                    ,vlp.Description  as darPoolDescription
                                    ,pb.poolSupply as poolBalance
                                    ,'USD'  as poolQuoteCurrency
                                    ,pa.tokenDARTicker
                                    ,pa.tokenDARAssetID
                                    ,pa.tokenBalance
                                    ,pa.tokenPrice
                                    ,pa.tokenValueInPool
                                    ,pa.tokenQuoteCurrency
                                    ,pa.WindowStart as windowStart
                                    ,pa.WindowEnd as windowEnd
                                    ,pa.EffectiveTime as effectiveTime
                                    ,'DAR-LP-Latest' as methodologyCode
                            from (
                                select pb.darPoolID
                                ,last(pb.poolSupply,timestampUTC) as poolSupply
                                from daxanddex.lpPoolBalance pb
                                where pb.darPoolID in (IDENTIFIERS)
                                  and pb.timestampUTC > '{windowStart}'
                                  and pb.timestampUTC <= '{windowEnd}'
                                group by pb.darPoolID
                            ) pb 
                            inner join refmaster_public.vLiquidityPool vlp  on pb.darPoolID = vlp.DARLiqPoolID
                            inner join refmaster_public.vLiquidityPoolContracts vlpc on pb.darPoolID = vlpc.DARLiqPoolID and vlpc.UseForSupply = 1
                            inner join (
                                    select lpb.darPoolID,upper(ticker) as tokenDARTicker,darAssetID as tokenDARAssetID,last(tokenSupply,timestampUTC)  as tokenBalance, last(usd_price,from_unixtime(p.effective_time)) as tokenPrice,tokenBalance * tokenPrice as tokenValueInPool, 'USD' as tokenQuoteCurrency
                                    ,REPLACE(DATE_FORMAT(last(from_unixtime(p.effective_time),from_unixtime(p.effective_time)), '%Y-%m-%dT%TZ'),'Z','+00:00') as EffectiveTime
                                    ,REPLACE(DATE_FORMAT(last(from_unixtime(p.window_start),from_unixtime(p.window_start)), '%Y-%m-%dT%TZ'),'Z','+00:00')  as WindowStart
                                    ,REPLACE(DATE_FORMAT(last(from_unixtime(p.window_end),from_unixtime(p.window_end)), '%Y-%m-%dT%TZ'),'Z','+00:00') as WindowEnd
                                    from daxanddex.lpTokenBalance lpb 
                                    inner join {DARApplicationInfo.CalcPriceDatabase}.vAll_full_window_price p on lpb.darAssetID = p.dar_identifier
                                    where darPoolID in (IDENTIFIERS)
                                    and timestampUTC > '{windowStart}'
                                    and timestampUTC <= '{windowEnd}'
                                    and from_unixtime(p.effective_time) > '{windowStart}'
                                    and from_unixtime(p.effective_time) <= '{windowEnd}'
                                    group by lpb.darPoolID,darAssetID,ticker
                            ) pa on pb.darPoolID = pa.darPoolID 
                            
                            ";

            sql = sql.Replace("IDENTIFIERS", identifiers);


            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                lAll = connection.Query<LiquidityPoolViewModel>(sql).ToList();
            }


            List<LiquidityPoolViewModel> finalResult = new List<LiquidityPoolViewModel>();
            string[] poolIds = identifiers.Replace("'","").Split(',');
            foreach (var id in poolIds)
            {

                LiquidityPoolViewModel result = new LiquidityPoolViewModel();

                var l = lAll.Where(x => x.DarPoolID == id).ToList();


                if (l.Any())
                {
                    result.MethodologyCode = l.ElementAtOrDefault(0).MethodologyCode;
                    result.DarPoolID = l.ElementAtOrDefault(0).DarPoolID;
                    result.DarPoolTicker = l.ElementAtOrDefault(0).DarPoolTicker;
                    result.DarPoolName = l.ElementAtOrDefault(0).DarPoolName;
                    result.DarPoolDescription = l.ElementAtOrDefault(0).DarPoolDescription;
                    result.PoolBalance = l.ElementAtOrDefault(0).PoolBalance;
                    result.PoolQuoteCurrency = l.ElementAtOrDefault(0).PoolQuoteCurrency;
                    result.EffectiveTime = l.Where(x => x.DarPoolID.Equals(id)).Max(d => d.EffectiveTime); 

                    result.PoolValueTotal = l.Where(x => x.DarPoolID.Equals(id)).Sum(d => d.tokenValueInPool);
                    result.PoolTokenPrice = result.PoolValueTotal / result.PoolBalance;


                    result.PoolAssets = l.Select(f => new PoolAssets() { TokenDARTicker = f.tokenDARTicker, TokenDARAssetID = f.tokenDARAssetID, TokenBalance = f.tokenBalance, TokenPrice = f.tokenPrice, TokenValueInPool = f.tokenValueInPool, TokenQuoteCurrency = f.tokenQuoteCurrency, WindowStart = f.windowStart, WindowEnd = f.windowEnd }).Cast<PoolAssets>();
                }
                else
                {
                    result.Errors = new List<string>() { $@"Data is not available for input parameter darPoolIdentifier:{id}
                        quoteCurrency:{quoteCurrency} windowStart:{windowStart} windowEnd:{windowEnd} methodology:{methodology}" };

                }

                finalResult.Add(result);
            }

            return finalResult;
        }

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

        public override DARViewModel Get(string key)
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