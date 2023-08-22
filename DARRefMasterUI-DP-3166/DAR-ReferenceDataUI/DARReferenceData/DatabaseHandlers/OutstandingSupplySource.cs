using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using Google.Protobuf.WellKnownTypes;
using log4net;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DARReferenceData.DatabaseHandlers
{
    public class OutstandingSupplySource : RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        public override long Add(DARViewModel i)
        {
            var a = (OutstandingSupplySourceViewModel)i;
            a.CreateUser = string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.User.Identity.Name) ? Environment.UserName : System.Web.HttpContext.Current.User.Identity.Name;

            
            var asset = (new Asset()).Get(a.DARAssetID) as AssetViewModel;
            if (asset == null)
                throw new Exception($"Invalid asset {a.DARAssetID}");

            var source = (new Source()).Get(a.DARSourceID) as SourceViewModel;
            if (source == null)
                throw new Exception($"Invalid source {a.DARSourceID}");

            string amount = a.ManualValue.ToString();
            if (string.IsNullOrWhiteSpace(amount))
                amount = "null";


           

            a.ShortName = source.ShortName;
            a.Creator = a.CreateUser;
            a.Operation = "INSERT";
            a.Deleted = 0;
            string publishstatus = SourceTablePublish(a);

            //string query = $@"insert into {DARApplicationInfo.SingleStoreCatalogInternal}.circulatingsupplysource
            //                (DARAssetID,darsourceid,ShortName,ManualValue,LoadTimestamp,CreateUser) 
            //                values ('{asset.DARAssetID}','{source.DARSourceID}','{source.ShortName}',{amount},now(), '{a.CreateUser}')
            //                ";

            
            //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //{
            //    var result = (IDictionary<string, object>)connection.Query<object>(query).FirstOrDefault();
            //}
            return 0;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (OutstandingSupplySourceViewModel)i;

            a.Creator = a.CreateUser;
            a.LastEditUser = string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.User.Identity.Name) ? Environment.UserName : System.Web.HttpContext.Current.User.Identity.Name;
            a.Operation = "DELETE";
            a.Deleted = 1;

            
            string publishstatus = SourceTablePublish(a);

            //string query = $@"DELETE FROM {DARApplicationInfo.SingleStoreCatalogInternal}.circulatingsupplysource
            //                  WHERE unix_timestamp(LoadTimestamp) = '{a.LoadTimestamp}'";

            //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //{
            //    var updatedCount = connection.Execute(query);
            //}

            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<OutstandingSupplySourceViewModel> l = new List<OutstandingSupplySourceViewModel>();

            string sql = $@"select CONCAT(o.DARAssetID, o.DARSourceID) as supplysourceID
                            ,o.DARAssetID 
                            ,a.DARTicker
                            ,o.DARSourceID
                            ,s.ShortName
                            ,s.SourceType
                            ,o.ManualValue
                            ,unix_timestamp(o.LoadTimestamp) as LoadTimestamp
                            ,o.CreateUser as Creator
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.circulatingsupplysource o 
                            inner join {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on a.Darassetid = o.darassetid
                            inner join {DARApplicationInfo.SingleStoreCatalogInternal}.vSource s on o.darsourceid = s.darsourceid
                            where o.Deleted = 0
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<OutstandingSupplySourceViewModel>(sql).ToList();
            }

            return l;
        }

        public DARViewModel GetCurrentRecord(string key)
        {
            List<OutstandingSupplySourceViewModel> l = new List<OutstandingSupplySourceViewModel>();

            string sql = $@"select CONCAT(o.DARAssetID, o.DARSourceID) as supplysourceID
                            ,o.DARAssetID 
                            ,a.DARTicker
                            ,o.DARSourceID
                            ,s.ShortName
                            ,s.SourceType
                            ,o.ManualValue
                            ,unix_timestamp(o.LoadTimestamp) as LoadTimestamp
                            ,o.CreateUser as Creator
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.circulatingsupplysource o 
                            inner join {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on a.Darassetid = o.darassetid
                            inner join {DARApplicationInfo.SingleStoreCatalogInternal}.vSource s on o.darsourceid = s.darsourceid
                            where  CONCAT(o.DARAssetID, o.DARSourceID) = '{key}' and o.deleted = 0
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<OutstandingSupplySourceViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }



        public IEnumerable<DARViewModel> GetRawCirculatingSupply()
        {
            List<OutstandingSupplyRawViewModel> l = new List<OutstandingSupplyRawViewModel>();

            string sql = $@"select
                            *
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.rawCirculatingSupply o
                            where LoadTS >  DATE_ADD(now(), INTERVAL -8 HOUR) 
                            order by LoadTS desc
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<OutstandingSupplyRawViewModel>(sql).ToList();
            }

            return l;
        }

        public override DARViewModel Get(string key)
        {
            return Get().Cast<OutstandingSupplyViewModel>().Where(x => x.darAssetID.Equals(key)).FirstOrDefault();
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            var a = (OutstandingSupplySourceViewModel)i;

            var current_record = GetCurrentRecord(a.supplysourceID);

            Delete(current_record);
            Add(a);
                        
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

        public string SourceTablePublish(OutstandingSupplySourceViewModel l)
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
            l.LoadTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();


            string jsondata = JsonSerializer.Serialize(l);
            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    producer.Produce("circulating_supply_source", new Message<Null, string> { Value = jsondata });
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