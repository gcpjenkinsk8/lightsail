using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class UrlType : RefDataHandler
    {

        public override long Add(DARViewModel i)
        {
            var a = (UrlTypeViewModel)i;

            DateTime updateTime = DateTime.Now.ToUniversalTime();

           
            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.IsActive = true;

            a.Operation = "INSERT";
            a.Deleted = 0;

            if (string.IsNullOrEmpty(a.DARURLTypeID))
            {
                a.DARURLTypeID = GetNextId();
            }

            if (string.IsNullOrEmpty(a.Name) || string.IsNullOrEmpty(a.DisplayName))
            {
                throw new Exception("Name and display Name cannot be left blank.");
            }

            var duplicate_name = GetTheExistingRecord(a.Name, "Name");
            var duplicate_display_name = GetTheExistingRecord(a.DisplayName, "DisplayName");

            if (duplicate_name != null && !string.Equals(duplicate_name.DARURLTypeID, a.DARURLTypeID))
            {
                throw new Exception("Url with same name is already present in the DB.");
            }

            if (duplicate_display_name != null && !string.Equals(duplicate_display_name.DARURLTypeID, a.DARURLTypeID))
            {
                throw new Exception("Url with same dislay name is already present in the DB.");
            }

            a.IsActive = true;
            string publishstatus = URLTypePublish(a);


            return 0;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (UrlTypeViewModel)i;

            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.IsActive = true;

            a.Operation = "DELETE";
            a.Deleted = 1;
            a.IsActive = true;
            string publishstatus = URLTypePublish(a);



            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<UrlTypeViewModel> l = new List<UrlTypeViewModel>();

            string sql = $@"
                            SELECT DARURLTypeID
                                  , Name
                                  , DisplayName
                                  , APIName
                                  , IsActive
                                  , CreateUser
                                  , LastEditUser
                                  , CreateTime
                                  , LastEditTime
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.URLType where DELETED = 0";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<UrlTypeViewModel>(sql).ToList();
            }

            return l;
        }

        public override DARViewModel Get(string key)
        {
            return Get().Cast<UrlTypeViewModel>().Where(x => x.Name.ToUpper().Equals(key.ToUpper())).FirstOrDefault();
        }

        public  DARViewModel GetOlderRecord(string key)
        {
            return Get().Cast<UrlTypeViewModel>().Where(x => x.DARURLTypeID.ToUpper().Equals(key.ToUpper())).FirstOrDefault();
        }

        public UrlTypeViewModel GetTheExistingRecord(string values, string operation)
        {
            List<UrlTypeViewModel> r = new List<UrlTypeViewModel>();
            string sql = $@"
                            select *
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.URLType
                            where {operation} = '{values}' and deleted = 0
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                r = connection.Query<UrlTypeViewModel>(sql).ToList();

                return r.FirstOrDefault();
            }
        }


        public override string GetNextId()
        {
            return GetNextId("DURLT", 5, 100);
        }


        public override bool IdExists(string nextId)
        {
            string sql = $@"
                            select DARURLTypeID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.URLType
                            where DARURLTypeID = '{nextId}'
                            
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
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
            var a = (UrlTypeViewModel)i;

            UrlTypeViewModel older_record = (UrlTypeViewModel)GetOlderRecord(a.DARURLTypeID); 

            Delete(older_record);
            try
            {
                Add(a);
            }
            catch (Exception e)
            {
                older_record.Deleted = 0;
                URLTypePublish(older_record);
                throw new Exception(e.Message);

            }

            return true;
        }

        public string URLTypePublish(UrlTypeViewModel l)
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
                    producer.Produce("url_type", new Message<Null, string> { Value = jsondata });
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