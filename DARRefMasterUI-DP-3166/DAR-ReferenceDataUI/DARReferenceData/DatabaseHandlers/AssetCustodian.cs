using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Reflection;
using System.Web;
using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;

namespace DARReferenceData.DatabaseHandlers
{
    public class AssetCustodian : RefDataHandler
    {
        public AssetCustodianViewModel Kafka_message_create(AssetCustodianViewModel a)
        {
            AssetCustodianViewModel l = new AssetCustodianViewModel()
            {
                DARAssetID = a.DARAssetID,
                AssetName = a.AssetName,
                DARCustodianID = a.DARCustodianID,
                Custodian = a.Custodian,
                CreateTime = a.CreateTime,
                CreateUser = a.CreateUser,
                LastEditTime = a.LastEditTime,
                LastEditUser = a.LastEditUser,
                Operation = a.Operation,
                Deleted = a.Deleted,
            };
            
            return l;
        }

        public override long Add(DARViewModel i)
        {
            var a = (AssetCustodianViewModel)i;


            if (string.IsNullOrWhiteSpace(a.DARAssetID))
            {
                throw new Exception($"Input asset is required");
            }

            if (!string.IsNullOrWhiteSpace(a.DARAssetID))
            {
                var o = (new Asset()).Get(string.IsNullOrWhiteSpace(a.AssetName) ? a.DARAssetID : a.AssetName);
                if (o == null)
                {
                    throw new Exception($"Invalid Asset: {a.AssetName}");
                }
                a.DARAssetID = ((AssetViewModel)o).DARAssetID;
            }

            if (string.IsNullOrWhiteSpace(a.DARCustodianID))
            {
                var o = (new Custodian()).Get(a.Custodian);
                if (o == null)
                {
                    throw new Exception($"Invalid Custodian: {a.Custodian}");
                }
                a.DARCustodianID = ((CustodianViewModel)o).DARCustodianID;
            }
            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            a.Operation = "INSERT";
            a.Deleted = 0;
            string publishstatus = AssetCustodianPublish(a);

            return 1;

            //string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLAssetCustodian
            //                ( 'UPSERT'
            //                ,@AssetID
            //                , @CustodianID
            //                , @CreateUser
            //                , @LastEditUser
            //                , @ID)";

            //var p = new DynamicParameters();
            //Type t = a.GetType();
            //PropertyInfo[] props = t.GetProperties();

            //foreach (var prop in props)
            //{
            //    var value = prop.GetValue(a, new object[] { });
            //    p.Add($"@{prop.Name}", value);
            //}

            // p.Add("@createTime", updateTime);
            // p.Add("@lastEditTime", updateTime);

            //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //{
            //    var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
            //    if (result.Any())
            //    {
            //        long iid = (long)result.Values.FirstOrDefault();
            //        a.ID = iid;
            //    }
            //}

            //return a.ID;
        }

        public bool Delete(string assetId)
        {
            string cmd = $@"DELETE FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AssetCustodian WHERE AssetID = @AssetID";

            int affectedRows;
            using (IDbConnection connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                affectedRows = connection.Execute(cmd, new { AssetID = assetId });
            }

            return affectedRows > 0;
        }

        public void Delete(string custodian, string assetId)
        {
            string sql = $@"DELETE FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AssetCustodian
                            WHERE DARAssetID=@AssetID AND CustodianID=(SELECT ID FROM Custodian WHERE Name=@Name);";

            using (IDbConnection connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                connection.Execute(sql, new { AssetID = assetId, Name = custodian });
            }
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (AssetCustodianViewModel)i;

            a.Operation = "DELETE";
            a.Deleted = 1;
            var kafka_message = Kafka_message_create(a);
            string publishstatus = AssetCustodianPublish(kafka_message);

            return true;

            //return Delete(a.DARAssetID);
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<AssetCustodianViewModel> l = new List<AssetCustodianViewModel>();

            string sql = $@"
                            SELECT a.DARAssetID
	                               ,a.Name as AssetName
	                              ,th.Name as Custodian
                                  ,t.DARCustodianID
                                  ,t.CreateUser
                                  ,t.LastEditUser
                                  ,t.CreateTime
                                  ,t.LastEditTime
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AssetCustodian t
                              INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on t.DARAssetID = a.DARAssetID
                              INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Custodian th on t.DARCustodianID = th.DARCustodianID
                              where t.Deleted = 0

                          ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<AssetCustodianViewModel>(sql).ToList();
            }

            return l;
        }

        public List<AssetCustodianViewModel> GetAssetCustodianList(string darAssetID)
        {
            List<AssetCustodianViewModel> l = new List<AssetCustodianViewModel>();
            string sql = $@"
                              SELECT 
                                  concat(t.DARCustodianID,t.DARAssetID) as ID
                                  ,t.DARCustodianID
                                  ,t.DARAssetID
                                  ,a.Name as AssetName
                                  ,th.Name as Custodian
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AssetCustodian t
                              INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Custodian th on t.DARCustodianID = th.DARCustodianID
                              INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on t.DARAssetID = a.DARAssetID
                              where th.Deleted = 0 and t.deleted = 0
                          ";

            var p = new DynamicParameters();
            p.Add("@DARAssetID", darAssetID);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<AssetCustodianViewModel>(sql, p).ToList();
            }
            return l;
        }

        public Dictionary<string, bool> GetAssetCustodians(string darAssetID)
        {
            List<AssetCustodianViewModel> l = new List<AssetCustodianViewModel>();
            string sql = $@"
                            SELECT th.Name as Custodian
                                  ,0 as IsActive
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AssetCustodian t
                              INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Custodian th on t.DARCustodianID = th.DARCustodianID
                              WHERE t.DARAssetID = @DARAssetID
                              and t.deleted = 0
                          ";

            var p = new DynamicParameters();
            p.Add("@DARAssetID", darAssetID);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<AssetCustodianViewModel>(sql, p).ToList();
            }
            return l.ToDictionary(x => x.Custodian, x => x.IsActive);
        }

        public bool CustodianExists(string assetId, string custodianId)
        {
            string sql = $@"
                            SELECT count(*)
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AssetCustodian t
                              WHERE t.DARAssetId = @assetId and t.DARCustodianID = @custodianId and deleted = 0
                          ";

            var p = new DynamicParameters();
            p.Add("@assetId", assetId);
            p.Add("@custodianId", custodianId);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var l = connection.Query<int>(sql, p).FirstOrDefault();

                if (l > 0)
                    return true;
            }

            return false;
        }


        public DARViewModel Get(string asset, string custodian)
        {
            if (string.IsNullOrWhiteSpace(asset))
                return null;
            if (string.IsNullOrWhiteSpace(custodian))
                return null;

            return Get().Cast<AssetCustodianViewModel>().Where(x => x.DARAssetID.ToUpper().Equals(asset.ToUpper()) && x.DARCustodianID.ToUpper().Equals(custodian)).FirstOrDefault();
        }

        public IEnumerable<DARViewModel> GetList(string asset)
        {
            if (string.IsNullOrWhiteSpace(asset))
                return null;

            List<AssetCustodianViewModel> l = new List<AssetCustodianViewModel>();

            string sql = $@"
                            SELECT a.DARAssetID
	                               ,a.Name as AssetName
	                              ,th.Name as Custodian
                                  ,t.DARCustodianID
                                  ,t.CreateUser
                                  ,t.LastEditUser
                                  ,t.CreateTime
                                  ,t.LastEditTime
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AssetCustodian t
                              INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on t.DARAssetID = a.DARAssetID
                              INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Custodian th on t.DARCustodianID = th.DARCustodianID
                              where t.Deleted = 0 and t.DARAssetID = '{asset}'

                          ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<AssetCustodianViewModel>(sql).ToList();
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
            DateTime updateTime = DateTime.Now.ToUniversalTime();

            var a = (AssetCustodianViewModel)i;
            var old_custodian_id = a.DARCustodianID;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.ID = string.Empty;
            if (string.IsNullOrWhiteSpace(a.AssetName))
            {
                throw new Exception($"Invalid AssetName:{a.AssetName}");
            }

            if (!string.IsNullOrWhiteSpace(a.AssetName))
            {
                var oan = (AssetViewModel)(new Asset()).Get(a.AssetName);
                if (oan != null)
                    a.DARAssetID = oan.DARAssetID;
            }

            if (string.IsNullOrWhiteSpace(a.DARAssetID))
            {
                throw new Exception($"Can't lookup Asset using AssetName:{a.AssetName}");
            }

            a.DARCustodianID = string.Empty;
            if (string.IsNullOrWhiteSpace(a.Custodian))
            {
                throw new Exception($"Invalid Custodian Name:{a.Custodian}");
            }

            var o = (CustodianViewModel)(new Custodian()).Get(a.Custodian);
            if (o != null)
                a.DARCustodianID = o.DARCustodianID;
            else
            {
                throw new Exception($"Can't lookup Custodian using Name:{a.Custodian}. Please make sure that this Custodian exists in DAR Database");
            }


            a.DARCustodianID = old_custodian_id;

            Delete(a);

            a.DARCustodianID = o.DARCustodianID;
            Add(a);
            

            return true;

            //string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLAssetCustodian
            //                ( 
            //                'UPSERT'
            //                , @AssetID
            //                , @CustodianID
            //                , @CreateUser
            //                , @LastEditUser
            //                , @ID)";

            //var p = new DynamicParameters();
            //Type t = a.GetType();
            //PropertyInfo[] props = t.GetProperties();

            //foreach (var prop in props)
            //{
            //    var value = prop.GetValue(a, new object[] { });
            //    p.Add($"@{prop.Name}", value);
            //}

            //long recordId = 0;

            //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //{
            //    var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
            //    recordId = (long)result.Values.FirstOrDefault();
            //}

            //return recordId == a.ID;
        }

        public override bool IdExists(string nextId)
        {
            throw new NotImplementedException();
        }

        public override string GetNextId()
        {
            throw new NotImplementedException();
        }

        public string AssetCustodianPublish(AssetCustodianViewModel l)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = DARApplicationInfo.KafkaServerName,
                ClientId = Dns.GetHostName(),
                EnableIdempotence = false,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = DARApplicationInfo.KafkaSaslUsername,
                SaslPassword = DARApplicationInfo.KafkaPassword,
                SslCaLocation = DARApplicationInfo.KafkaCertificateFileName,
                BatchNumMessages = 1,
                BatchSize = 1000000,

            };


            string jsondata = JsonSerializer.Serialize(l);
            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    producer.Produce("asset_custodian", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }
    }
}