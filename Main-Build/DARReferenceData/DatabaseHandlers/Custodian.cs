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
    public class Custodian : RefDataHandler
    {

        public CustodianViewModel CurrentCustodian { get; set; }

        public Custodian()
        {
        }

        public Custodian(string code)
        {
            if (!string.IsNullOrWhiteSpace(code))
                Get(code);
        }

        public override long Add(DARViewModel i)
        {
            var a = (CustodianViewModel)i;

            DateTime updateTime = DateTime.Now.ToUniversalTime();

            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.IsActive = true;

            if (string.IsNullOrWhiteSpace(a.DARCustodianID))
            {
                a.DARCustodianID = GetNextId();
            }

            a.Operation = OPS_INSERT;
            a.Deleted = 0;
            string publishstatus = Publish(a);


            return 1;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (CustodianViewModel)i;
            DateTime updateTime = DateTime.Now.ToUniversalTime();

            a.Operation = "DELETE";
            a.Deleted = 1;
            string publishstatus = Publish(a);


            
            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<CustodianViewModel> l = new List<CustodianViewModel>();

            string sql = $@"
                            SELECT DARCustodianID
                                  , Name
                                  , IsActive
                                  , CreateUser
                                  , LastEditUser
                                  , CreateTime
                                  , LastEditTime
                                  , Description
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Custodian where DELETED = 0";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<CustodianViewModel>(sql).ToList();
            }

            return l;
        }

        public override DARViewModel Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            CurrentCustodian = Get().Cast<CustodianViewModel>().Where(x => x.Name.ToUpper().Equals(key.ToUpper())).FirstOrDefault();
            return CurrentCustodian;
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            var a = (CustodianViewModel)i;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            //Delete(a);
            Add(a);

            return true;
        }

        public override bool IdExists(string nextId)
        {
            string sql = $@"
                            select DARCustodianID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Custodian
                            where DARCustodianID = '{nextId}'
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                var r = connection.Query<string>(sql);

                if (r != null && r.Any())
                    return true;
            }

            return false;
        }

        public override string GetNextId()
        {
            return GetNextId("DCUST", 5, 100);
        }

        public string Publish(CustodianViewModel l)
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
                    producer.Produce("custodian", new Message<Null, string> { Value = jsondata });
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