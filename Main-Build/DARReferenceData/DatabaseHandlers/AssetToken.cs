using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class AssetToken : RefDataHandler
    {
        public AssetTokenViewModel Kafka_message_create(AssetTokenViewModel a)
        {
            AssetTokenViewModel l = new AssetTokenViewModel()
            {
                DARAssetID = a.DARAssetID,
                AssetName = a.AssetName,
                TokenId = a.TokenId,
                BlockChainId = a.BlockChainId,
                AssetID = a.AssetID,
                DARTicker = a.DARTicker,
                DARTokenID = a.DARTokenID,
                TokenName = a.TokenName,
                TokenContractAddress = a.TokenContractAddress,
                ConsensusMechanism = a.ConsensusMechanism,
                HashAlgorithm = a.HashAlgorithm,
                BlockChain = a.BlockChain,
                CreateTime = a.CreateTime,
                CreateUser = a.CreateUser,
                LastEditTime = a.LastEditTime,
                LastEditUser = a.LastEditUser,
                Operation = a.Operation,
                Deleted = a.Deleted,
            };
             
            return l;
        }

        public AssetTokenViewModel CurrentAssetToken { get; set; }

        public AssetToken(string assetId, string tokenId, string blockchainId)
        {
            CurrentAssetToken = Get().Cast<AssetTokenViewModel>().Where(x => x.AssetID.Equals(assetId) &&
                                                x.TokenId.Equals(tokenId) &&
                                                x.BlockChainId.Equals(blockchainId)
                                                 ).FirstOrDefault();
        }

        public AssetToken()
        {
        }

        public override long Add(DARViewModel i)
        {
            var a = (AssetTokenViewModel)i;

            /*
                        string sql = $@"
                                        INSERT INTO [{CatalogName}].{SchemaName}.[AssetToken]
                                               ([AssetID]
                                                    ,[TokenId]
                                                    ,[BlockChainId]
                                                    ,[TokenContractAddress]
                                                    ,[IsActive]
                                                    ,[CreateUser]
                                                    ,[CreateTime])
                                                VALUES
                                                    (@AssetID
                                                    ,@TokenId
                                                    ,@BlockChainId
                                                    ,@TokenContractAddress
                                                    ,@IsActive
                                                    ,@CreateUser
                                                    ,@CreateTime)

                                           SELECT CAST(SCOPE_IDENTITY() as int)
                                                                   ";
            */

            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            a.Operation = "INSERT";
            a.Deleted = 0;
            var kafka_message = Kafka_message_create(a);
            string publishstatus = AssetTokenPublish(kafka_message);


            //string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLAssetToken
            //    ('UPSERT'
            //    , @AssetID
            //    , @TokenId
            //    , @BlockChainId
            //    , @TokenContractAddress
            //    , @CreateUser
            //    , @LastEditUser
            //    , @ID
            //    , @IsActive
            //    )";

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

            //return iid;
            return 1;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (AssetTokenViewModel)i;
            DateTime updateTime = DateTime.Now.ToUniversalTime();

            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            a.Operation = "DELETE";
            a.Deleted = 1;
            var kafka_message = Kafka_message_create(a);
            string publishstatus = AssetTokenPublish(kafka_message);

            //string sql = $@"DELETE {DARApplicationInfo.SingleStoreCatalogInternal}.AssetToken
            //               WHERE ID = @Id

            //                DELETE {DARApplicationInfo.SingleStoreCatalogInternal}.Token
            //               WHERE DARTokenID = @DARTokenID
            //                ";

            //string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLAssetToken
            //    ('DELETE'
            //    , @AssetID
            //    , @TokenId
            //    , @BlockChainId
            //    , @TokenContractAddress
            //    , @CreateUser
            //    , @LastEditUser
            //    , @ID
            //    , @IsActive
            //    )";

            //var p = new DynamicParameters();
            //foreach (var prop in a.GetType().GetProperties())
            //{
            //    object value = prop.GetValue(a, new object[] { });
            //    p.Add($"@{prop.Name}", value);
            //}

            //long deleteId = 0;

            //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //{
            //    var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
            //    deleteId = (long)result.Values.FirstOrDefault();
            //}

            //return deleteId == a.ID;
            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<AssetTokenViewModel> l = new List<AssetTokenViewModel>();

            string sql = $@"
                            SELECT ID
                                  , AssetID
                                  , TokenId
                                  , BlockChainId
                                  , TokenContractAddress
                                  , IsActive
                                  , CreateUser
                                  , LastEditUser
                                  , CreateTime
                                  , LastEditTime
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AssetToken
                           ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<AssetTokenViewModel>(sql).ToList();
            }

            return l;
        }

        public IEnumerable<DARViewModel> GetAssetTokenView()
        {
            List<AssetTokenViewModel> l = new List<AssetTokenViewModel>();

            string sql = $@"
                            SELECT *
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.vAssetToken
                           ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<AssetTokenViewModel>(sql).ToList();
            }

            return l;
        }

        public override DARViewModel Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            CurrentAssetToken = Get().Cast<AssetTokenViewModel>().Where(x => x.TokenContractAddress.ToUpper().Equals(key.ToUpper())).FirstOrDefault();
            return CurrentAssetToken;
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            var a = (AssetTokenViewModel)i;

            Delete(a);
            Add(a);

            /*            DateTime updateTime = DateTime.Now.ToUniversalTime();

                        string sql = $@"
                                        UPDATE {DARApplicationInfo.SingleStoreCatalogInternal}.AssetToken
                                           SET TokenContractAddress = @TokenContractAddress
                                              , IsActive = @IsActive
                                              , LastEditUser = @LastEditUser
                                              , LastEditTime = @LastEditTime
                                         WHERE ID = @Id";

            */

            //string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spdDMLAssetToken
            //    ('UPSERT'
            //    , @AssetID
            //    , @TokenId
            //    , @BlockChainId
            //    , @TokenContractAddress
            //    , @CreateUser
            //    , @LastEditUser
            //    , @ID
            //    , @IsActive
            //    )";

            //var p = new DynamicParameters();
            //foreach (var prop in a.GetType().GetProperties())
            //{
            //    object value = prop.GetValue(a, new object[] { });
            //    p.Add($"@{prop.Name}", value);
            //}

            //p.Add("@LastEditUser", string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name);

            //long recordId = 0;

            //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //{
            //    var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
            //    recordId = (long)result.Values.FirstOrDefault();
            //}

            //return recordId == a.ID;
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

        public string AssetTokenPublish(AssetTokenViewModel l)
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
                    producer.Produce("asset_token", new Message<Null, string> { Value = jsondata });
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