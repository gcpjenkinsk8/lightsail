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
    public class ExchangePair : RefDataHandler
    {
        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public bool ExchangePairExists(string exchangeId, string pair)
        {
            var p = Get().Cast<ExchangePairViewModel>().Where(x => x.ExchangePairName.Equals(pair) && x.DARExchangeId.Equals(exchangeId)).FirstOrDefault();
            if (p == null)
                return false;
            else
                return true;
        }

        public ExchangePairViewModel GetExchangePair(string exchangeId, string pair)
        {
            var p = Get().Cast<ExchangePairViewModel>().Where(x => x.ExchangePairName.Equals(pair) && x.DARExchangeId.Equals(exchangeId)).FirstOrDefault();
            return p;
        }

        public bool UpsertExchangePair(ExchangePairViewModel vm, out string errors)
        {
            errors = string.Empty;

            //Asset a = new Asset(vm.DARAssetId);
            //Asset c = new Asset(vm.DARCurrencyId);

            //if (a.CurrentAsset == null)
            //{
            //    errors = errors + $" Invalid Asset {vm.DARAssetId}";
            //}
            //if (c.CurrentAsset == null)
            //{
            //    errors = errors + $" Invalid Currency Asset {vm.DARCurrencyId}";
            //}

            //Exchange e = new Exchange(vm.DARExchangeId);
            //if (e.CurrentExchange == null)
            //{
            //    errors = errors + $" Invalid Exchange {vm.DARExchangeId}";
            //}

            //if (a.CurrentAsset == null || c.CurrentAsset == null || e.CurrentExchange == null)
            //{
            //    return false;
            //}

            //vm.ExchangeID = e.CurrentExchange.DARExchangeID;

            //Pair p = new Pair();

            //PairViewModel pvm = p.PairExists(a.CurrentAsset.ID, c.CurrentAsset.ID);
            //if (pvm != null)
            //    vm.PairID = pvm.ID;
            //else
            //{
            //    pvm = new PairViewModel();
            //    pvm.AssetID = a.CurrentAsset.ID;
            //    pvm.QuoteAssetID = c.CurrentAsset.ID;
            //    pvm.DARName = $"{a.CurrentAsset.DARTicker}{c.CurrentAsset.DARTicker}";
            //    p.Add(pvm);
            //}

            //ExchangePair ep = new ExchangePair();
            //var existinggExchangePair = GetExchangePair(e.CurrentExchange.DARExchangeID, vm.ExchangePairName);

            //if (existinggExchangePair != null)
            //{
            //    vm.ID = existinggExchangePair.ID;
            //    Update(vm);
            //}
            //else
            //{
            //    Add(vm);
            //}
            return true;
        }

        public override long Add(DARViewModel i)
        {
            return 0;

            //var a = (ExchangePairViewModel)i;

            //string query = $@"CALL {DARApplicationInfo.SingleStoreInternalDB}. spDMLExchangePair(
            //            'UPSERT'
            //            , @pairID
            //            , @exchangeID
            //            , @exchangePairName
            //            , @ID
            //            , @exchangePairNumberId
            //            , @exchangePairStringId
            //            , @exchangePairShortName
            //            , @exchangePairLongName
            //            , @exchangeAssetStringId
            //            , @exchangeAssetNumberId
            //            , @exchangeAssetShortName
            //            , @exchangeAssetLongName
            //            , @exchangeCurrencyStringId
            //            , @exchangeCurrencyNumberId
            //            , @exchangeCurrencyShortName
            //            , @exchangeCurrencyLongName
            //            , @isAvailable
            //            , @createUser
            //            , @lastEditUser
            //            , @isActive
            //            )";

            //var p = new DynamicParameters();

            //p.Add("@pairID", a.PairID);
            //p.Add("@exchangeID", a.ExchangeID);
            //p.Add("@exchangePairName", a.ExchangePairName);
            //p.Add("@ID", a.ID);
            //p.Add("@isActive", a.IsActive);
            //p.Add("@createUser", Environment.UserName);
            //p.Add("@lastEditUser", Environment.UserName);

            //p.Add("@exchangePairNumberId", (object)a.ExchangePairNumberId ?? SqlString.Null);
            //p.Add("@exchangePairStringId", a.ExchangePairStringId);
            //p.Add("@exchangePairShortName", a.ExchangePairShortName);
            //p.Add("@exchangePairLongName", a.ExchangePairLongName);
            //p.Add("@exchangeAssetStringId", a.ExchangeAssetStringId);
            //p.Add("@exchangeAssetNumberId", (object)a.ExchangeAssetNumberId ?? SqlString.Null);
            //p.Add("@exchangeAssetShortName", a.ExchangeAssetShortName);
            //p.Add("@exchangeAssetLongName", a.ExchangeAssetLongName);
            //p.Add("@exchangeCurrencyStringId", a.ExchangeCurrencyStringId);
            //p.Add("@exchangeCurrencyNumberId", (object)a.ExchangeCurrencyNumberId ?? SqlString.Null);
            //p.Add("@exchangeCurrencyShortName", a.ExchangeCurrencyShortName);
            //p.Add("@exchangeCurrencyLongName", a.ExchangeCurrencyLongName);
            //p.Add("@isAvailable", (object)a.IsAvailable ?? SqlString.Null);

            //long iid = 0;

            //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //{
            //    var result = (IDictionary<string, object>)connection.Query<object>(query, p);
            //    iid = (long)result.Values.FirstOrDefault();
            //    if (!string.IsNullOrEmpty(result.Values.ElementAtOrDefault(1).ToString()) || result.Values.ElementAtOrDefault(1).ToString().Equals("Data Inserted"))
            //    {
            //        a.ID = iid;
            //    }
            //}

            //return iid;
        }

        public override bool Update(DARViewModel i)
        {
            return true;
            //var a = (ExchangePairViewModel)i;

            //string query = $@"CALL {DARApplicationInfo.SingleStoreInternalDB}. spDMLExchangePair(
            //            'UPSERT'
            //            , @pairID
            //            , @exchangeID
            //            , @exchangePairName
            //            , @ID
            //            , @exchangePairNumberId
            //            , @exchangePairStringId
            //            , @exchangePairShortName
            //            , @exchangePairLongName
            //            , @exchangeAssetStringId
            //            , @exchangeAssetNumberId
            //            , @exchangeAssetShortName
            //            , @exchangeAssetLongName
            //            , @exchangeCurrencyStringId
            //            , @exchangeCurrencyNumberId
            //            , @exchangeCurrencyShortName
            //            , @exchangeCurrencyLongName
            //            , @isAvailable
            //            , @createUser
            //            , @lastEditUser
            //            , @isActive
            //            )";

            //var p = new DynamicParameters();

            //p.Add("@pairID", a.PairID);
            //p.Add("@exchangeID", a.DARExchangeId);
            //p.Add("@exchangePairName", a.ExchangePairName);
            //p.Add("@ID", a.ID);
            //p.Add("@isActive", a.IsActive);
            //p.Add("@createUser", SqlString.Null);
            //p.Add("@lastEditUser", string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name);
            //// p.Add("@lastEditTime", updateTime);

            //p.Add("@exchangePairNumberId", (object)a.ExchangePairNumberId ?? SqlString.Null);
            //p.Add("@exchangePairStringId", a.ExchangePairStringId);
            //p.Add("@exchangePairShortName", a.ExchangePairShortName);
            //p.Add("@exchangePairLongName", a.ExchangePairLongName);
            //p.Add("@exchangeAssetStringId", a.ExchangeAssetStringId);
            //p.Add("@exchangeAssetNumberId", a.ExchangeAssetNumberId);
            //p.Add("@exchangeAssetShortName", a.ExchangeAssetShortName);
            //p.Add("@exchangeAssetLongName", a.ExchangeAssetLongName);
            //p.Add("@exchangeCurrencyStringId", a.ExchangeCurrencyStringId);
            //p.Add("@exchangeCurrencyNumberId", (object)a.ExchangeCurrencyNumberId ?? SqlString.Null);
            //p.Add("@exchangeCurrencyShortName", a.ExchangeCurrencyShortName);
            //p.Add("@exchangeCurrencyLongName", a.ExchangeCurrencyLongName);
            //p.Add("@isAvailable", (object)a.IsAvailable ?? SqlString.Null);

            //long recordId = 0;

            //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //{
            //    var res = (IDictionary<string, object>)connection.Query<object>(query, p);
            //    recordId = (long)res.Values.FirstOrDefault();
            //}

            //return recordId == a.ID;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (ExchangePairViewModel)i;
            DateTime updateTime = DateTime.Now.ToUniversalTime();

            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLExchangePair
                        ('DELETE'
                        , @ID)";

            var p = new DynamicParameters();
            p.Add("@Id", a.ID);

            int updatedCount = 0;
            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                updatedCount = connection.Execute(query, p);
            }

            if (updatedCount == 0)
                return false;

            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            var l = new List<ExchangePairViewModel>();

            string sql = $@"

                        SELECT ID
                              , PairID
                              , ExchangeID
                              , ExchangePairName
                              , IsActive
                              , CreateUser
                              , LastEditUser
                              , CreateTime
                              , LastEditTime
                          FROM {DARApplicationInfo.SingleStoreCatalogInternal}.ExchangePair";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ExchangePairViewModel>(sql).ToList();
            }

            return l;
        }

        public override DARViewModel Get(string key)
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