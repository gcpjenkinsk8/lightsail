using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class Pair : RefDataHandler
    {
        public static int GetCount()
        {
            string sql = $@"
                            select count(*)
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.exchangePairsV2
                              where DATE_FORMAT(endDate, '%Y-%m-%d') = '9999-12-31'
                    
                           ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                return connection.Query<int>(sql).FirstOrDefault();
            }
        }

        public PairViewModel PairExists(string assetId, string quoteAssetId)
        {
            return Get().Cast<PairViewModel>().Where(x => x.AssetID.Equals(assetId) && x.QuoteAssetID.Equals(quoteAssetId)).FirstOrDefault();
        }

        public string AddPair(DARViewModel i)
        {
            //var a = (PairViewModel)i;

            //DateTime updateTime = DateTime.Now.ToUniversalTime();
            ///*
            //            string sql = $@"INSERT INTO [{CatalogName}].{SchemaName}.[Pair]
            //                                       ([UniqueID]
            //                                       ,[AssetID]
            //                                       ,[QuoteAssetID]
            //                                       ,[DARName]
            //                                       ,[IsActive]
            //                                       ,[CreateUser]
            //                                       ,[LastEditUser]
            //                                       ,[CreateTime]
            //                                       ,[LastEditTime])
            //                            VALUES
            //                                (NewID()
            //                                ,@AssetID
            //                                ,@QuoteAssetID
            //                                ,@DARName
            //                                ,@IsActive
            //                                ,@CreateUser
            //                                ,@LastEditUser
            //                                ,@CreateTime
            //                                ,@LastEditTime)

            //                        SELECT CAST(SCOPE_IDENTITY() as int)
            //                        ";
            //*/

            //a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            //a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            //string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLPair
            //            ('UPSERT'
            //            , @AssetID
            //            , @QuoteAssetID
            //            , @DARName
            //            , @ID
            //            , @IsActive
            //            , @CreateUser
            //            , @LastEditUser
            //            )";

            //var p = new DynamicParameters();
            //foreach (var prop in a.GetType().GetProperties())
            //{
            //    object value = prop.GetValue(a, new object[] { });
            //    p.Add($"@{prop.Name}", value);
            //}

            //long iid = 0;

            //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //{
            //    var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
            //    iid = (long)result.Values.FirstOrDefault();

            //    if (iid > 0) a.ID = iid;
            //}

            return null;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (PairViewModel)i;
            DateTime updateTime = DateTime.Now.ToUniversalTime();
            /*
                        string sql = $@"
                                        DELETE [{CatalogName}].{SchemaName}.[ExchangePair] WHERE PairId = @Id and ExchangeID = @ExchangeId
                                        IF NOT EXISTS (SELECT * FROM ExchangePair where PairID = @Id)
                                        BEGIN
	                                        DELETE [{CatalogName}].{SchemaName}.[Pair] WHERE ID = @Id
                                        END

                                        ";
            */

            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLExchangePair
                        ('DELETE'
                        , @Id
                        , @ExchangeId
                        )";
            string query2 = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLPair
                        ('DELETE', @Id)";

            var p = new DynamicParameters();
            p.Add("@Id", a.ID);
            p.Add("@ExchangeId", a.SourceId);

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
            var l = new List<PairViewModel>();

            string sql = $@"
                           SELECT ID
                              , AssetID
                              , QuoteAssetID
                              , DARName
                              , IsActive
                              , CreateUser
                              , LastEditUser
                              , CreateTime
                              , LastEditTime
                          FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Pair";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<PairViewModel>(sql).ToList();
            }

            return l;
        }

        public IEnumerable<DARViewModel> GetPairView()
        {
            var l = new List<PairViewModel>();

            string sql = $@"
                          select *
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.vPair";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<PairViewModel>(sql).ToList();
            }

            return l;
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
            var a = (PairViewModel)i;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            /*
                        string sql = $@"UPDATE [{CatalogName}].{SchemaName}.[Pair]
                                       SET [AssetID] = @AssetID
                                          ,[QuoteAssetID] = @QuoteAssetID
                                          ,[DARName] = @DARName
                                          ,[IsActive] = @IsActive
                                          ,[LastEditUser] = @LastEditUser
                                          ,[LastEditTime] = @LastEditTime
                                     WHERE ID = @ID
                                    ";
            */
            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLPair
                        ('UPSERT'
                        , @AssetID
                        , @QuoteAssetID
                        , @DARName
                        , @ID
                        , @IsActive
                        , @CreateUser
                        , @LastEditUser
                        )";

            var p = new DynamicParameters();
            foreach (var prop in a.GetType().GetProperties())
            {
                object value = prop.GetValue(a, new object[] { });
                p.Add($"@{prop.Name}", value);
            }

            long recordId = 0;

            //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //{
            //    var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
            //    if (result.Any())
            //    {
            //        recordId = (long)result.Values.FirstOrDefault();
            //    }
            //}

            return true;
        }

        public override bool IdExists(string nextId)
        {
            throw new NotImplementedException();
        }

        public override string GetNextId()
        {
            throw new NotImplementedException();
        }

        public override long Add(DARViewModel i)
        {
            throw new NotImplementedException();
        }
    }
}