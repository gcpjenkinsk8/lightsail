using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class VettingStatus : RefDataHandler
    {
        public VettingStatusViewModel Kafka_message_create(VettingStatusViewModel a)
        {
            VettingStatusViewModel l = new VettingStatusViewModel()
            {
                StatusCode = a.StatusCode,
                StatusDescription = a.StatusDescription,
                StatusType = a.StatusType,
                Operation = a.Operation,
                Deleted = a.Deleted,
                CreateTime = a.CreateTime,
                CreateUser = a.CreateUser,
                LastEditTime = a.LastEditTime,
                LastEditUser = a.LastEditUser,
                IsActive = a.IsActive,
            };

            return l;
        }

        public override long Add(DARViewModel i)
        {
            var a = (VettingStatusViewModel)i;

            /*
                        string sql = $@"
                                        INSERT INTO {DARApplicationInfo.SingleStoreInternalDB}.VettingStatus
                                                     (StatusCode
                                                    ,StatusDescription
                                                    ,StatusType
                                                    ,IsActive
                                                    ,CreateUser
                                                    ,LastEditUser
                                                    ,CreateTime
                                                    ,LastEditTime)
                                                VALUES
                                                    (@StatusCode
                                                    ,@StatusDescription
                                                    ,@StatusType
                                                    ,@IsActive
                                                    ,@CreateUser
                                                    ,@LastEditUser
                                                    ,@CreateTime
                                                    ,@LastEditTime)
                                        SELECT CAST(SCOPE_IDENTITY() as int)
                                                                   ";
            */

            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            a.Operation = "INSERT";
            a.Deleted = 0;
            a.IsActive = 1;
            var kafka_message = Kafka_message_create(a);
            string publishstatus = VettingStatusPublish(kafka_message);


            //string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLVettingStatus(
            //                    'UPSERT'
            //                    , @StatusCode
            //                    , @StatusDescription
            //                    , @StatusType
            //                    , @ID
            //                    , @IsActive
            //                    , @CreateUser
            //                    , @LastEditUser
            //                    )";

            //var p = new DynamicParameters();

            //foreach (var prop in a.GetType().GetProperties())
            //{
            //    object value = prop.GetValue(a, new object[] { });
            //    p.Add($"@{prop.Name}", value);
            //}

            //p.Add("@IsActive", true);
            //p.Add("@CreateUser", string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name);
            //p.Add("@LastEditUser", string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name);

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
            var a = (VettingStatusViewModel)i;

            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            a.Operation = "DELETE";
            a.Deleted = 1;
            a.IsActive = 0;
            var kafka_message = Kafka_message_create(a);
            string publishstatus = VettingStatusPublish(kafka_message);



            //string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLVettingStatus(
            //                    'DELETE'
            //                    , @StatusCode
            //                    , @StatusDescription
            //                    , @StatusType
            //                    , @ID
            //                    , @IsActive
            //                    , @CreateUser
            //                    , @LastEditUser
            //                    )";

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
            List<VettingStatusViewModel> l = new List<VettingStatusViewModel>();

            string sql = $@"
                        SELECT StatusCode
                              , StatusDescription
                              , StatusType
                              , IsActive
                              , CreateUser
                              , LastEditUser
                              , CreateTime
                              , LastEditTime
                          FROM {DARApplicationInfo.SingleStoreCatalogInternal}.VettingStatus where DELETED = 0";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<VettingStatusViewModel>(sql).ToList();
            }

            return l;
        }

        public override DARViewModel Get(string key)
        {
            return Get().Cast<VettingStatusViewModel>().Where(x => x.StatusDescription.ToUpper().Equals(key.ToUpper())).FirstOrDefault();
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
            var a = (VettingStatusViewModel)i;

            Delete(a);
            Add(a);

            //string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLVettingStatus(
            //                    'UPSERT'
            //                    , @StatusCode
            //                    , @StatusDescription
            //                    , @StatusType
            //                    , @ID
            //                    , @IsActive
            //                    , @CreateUser
            //                    , @LastEditUser
            //                    )";

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

        public string VettingStatusPublish(VettingStatusViewModel l)
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
                    producer.Produce("vetting_status", new Message<Null, string> { Value = jsondata });
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