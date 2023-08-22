using Dapper;
using DARReferenceData.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace DARReferenceData.DatabaseHandlers
{
    public class ExchangePairs : RefDataHandler
    {
        public override long Add(DARViewModel i)
        {
            var a = (ExchangePairsViewModel)i;
            a.DARPairID = GetNextId();


            if (a.StartTime == a.EndTime)
            {
                a.EndTime = DateTime.MaxValue;
            }

            if (a.StartTime == DateTime.MinValue) a.StartTime = DateTime.Now;
            if (a.EndTime == DateTime.MinValue) a.EndTime = DateTime.MaxValue;

            if (a.EndTime.Year == 0000 || a.EndTime.Year == 10000)
                a.EndTime = DateTime.MaxValue;

            if (a.StartTime.Year == 0000)
                a.StartTime = DateTime.Today;




            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}. spDMLExchangePairs(
                        'INSERT'
                        , '{a.DARPairID}'
                        , '{a.DARExchangeID}'
                        , '{a.ExchangePair}'
                        , '{a.DARAssetID}'
                        , '{a.DARCurrencyID}'
                        , '{a.StartTime.ToString("yyyy-MM-dd HH:MM:ss.0")}'
                        , '{a.EndTime.ToString("yyyy-MM-dd HH:MM:ss.0")}'
                        , '{Environment.UserName}'
                        )";

            

            long iid = 0;


            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                ///var result = connection.Query<Dictionary<string,string>>(query);
                var result = (IDictionary<string, object>)connection.Query<object>(query).Single();

                if (!result.ElementAtOrDefault(1).Value.ToString().Equals("Inserted", StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new Exception($"Failed to insert row. {result.ElementAtOrDefault(1).Value.ToString()}");
                }
            }

            return iid;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (ExchangePairsViewModel)i;
            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}. spDMLExchangePairs(
                        'DELETE'
                        , '{a.DARPairID}'
                        , '{a.DARExchangeID}'
                        , '{a.ExchangePair}'
                        , '{a.DARAssetID}'
                        , '{a.DARCurrencyID}'
                        , '{a.StartTime}'
                        , '{a.EndTime}'
                        , '{Environment.UserName}'
                        )";




            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                ///var result = connection.Query<Dictionary<string,string>>(query);
                var result = (IDictionary<string, object>)connection.Query<object>(query).Single();

                if (!result.ElementAtOrDefault(1).Value.ToString().Equals("Deleted", StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new Exception("Failed to insert row");
                }
            }

            return true;
        }

        public IEnumerable<DARViewModel> GetExchangePairPublic()
        {
            var l = new List<ExchangePairsViewModel>();

            string sql = $@"
                            select darExchangeID as DARExchangeID
                                ,exchangePair as ExchangePair 
                                ,darAssetId as DARAssetID
                                ,darCurrencyID as DARCurrencyID
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.exchangePairs
                            ";


            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ExchangePairsViewModel>(sql).ToList();
            }

            return l;
        }


        public override IEnumerable<DARViewModel> Get()
        {
            var l = new List<ExchangePairsViewModel>();

            string sql = $@"
                        SELECT ep.*,a.NAME as Asset,c.Name as Currency,e.ShortName as Exchange
                        FROM {DARApplicationInfo.SingleStoreCatalogInternal}.exchangePairs ep
                        left join {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on ep.darAssetID = a.darAssetId
                        left join {DARApplicationInfo.SingleStoreCatalogInternal}.Asset c on ep.darCurrencyID = c.darAssetId
                        left join {DARApplicationInfo.SingleStoreCatalogInternal}.Exchange e on ep.darExchangeID = e.darExchangeID

                            ";


            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ExchangePairsViewModel>(sql).ToList();
            }

            return l;
        }

        public override DARViewModel Get(string key)
        {

            throw new NotImplementedException();
        }

        public override string GetNextId()
        {
            return GetNextId("DP", 9, 100);
        }

        public override bool IdExists(string nextId)
        {
            string sql = $@"

                            select DARPairID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.exchangePairs
                            where DARPairID = '{nextId}'
                        
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                var r = connection.Query<string>(sql);

                if (r != null && r.Any())
                    return true;
            }

            return false;
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {


            var a = (ExchangePairsViewModel)i;

            if (a.StartTime == a.EndTime)
            {
                a.EndTime = DateTime.MaxValue;
            }

            if (a.StartTime == DateTime.MinValue) a.StartTime = DateTime.Now;
            if (a.EndTime == DateTime.MinValue) a.EndTime = DateTime.MaxValue;

            if (a.EndTime.Year == 0000 || a.EndTime.Year == 10000)
                a.EndTime = DateTime.MaxValue;

            if (a.StartTime.Year == 0000)
                a.StartTime = DateTime.Today;


            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}. spDMLExchangePairs(
                        'UPDATE'
                        , '{a.DARPairID}'
                        , '{a.DARExchangeID}'
                        , '{a.ExchangePair}'
                        , '{a.DARAssetID}'
                        , '{a.DARCurrencyID}'
                        , '{a.StartTime.ToString("yyyy-MM-dd HH:MM:00.0")}'
                        , '{a.EndTime.ToString("yyyy-MM-dd HH:MM:00.0")}'
                        , '{Environment.UserName}'
                        )";



            
            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                ///var result = connection.Query<Dictionary<string,string>>(query);
                var result = (IDictionary<string, object>)connection.Query<object>(query).Single();

                if (!result.ElementAtOrDefault(1).Value.ToString().Equals("Updated", StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new Exception("Failed to insert row");
                }
            }

            return true;
        }


        public bool SynchWithServlist(DARViewModel i)
        {
  

            return true;
        }
    }
}
