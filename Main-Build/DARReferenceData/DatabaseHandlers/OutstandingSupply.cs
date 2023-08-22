using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using Google.Protobuf.WellKnownTypes;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DARReferenceData.DatabaseHandlers
{
    public class OutstandingSupply : RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        public static int GetCount()
        {
            int result = 0;

            try
            {
                string sql = $@"select count(*)
                                from {DARApplicationInfo.SingleStoreCatalogInternal}.OutstandingSupply
                                where CollectedTimeStamp >= timestamp(current_date()) and Deleted = 0";

                using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
                {
                    result = connection.Query<int>(sql).FirstOrDefault();
                }
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }

            return result;
        }

        public override long Add(DARViewModel i)
        {
            var a = (OutstandingSupplyViewModel)i;

            var circulatingSupplyExists = GetFinalizedOutStandingSupply(a.CreateTime).Cast<OutstandingSupplyViewModel>();
            var darAsset = from supply in circulatingSupplyExists
                           where supply.darAssetID.Equals(a.darAssetID)
                           where supply.ProcessID == a.ProcessID
                           where supply.DARSourceID == a.DARSourceID
                           where supply.Deleted == 0
                           select supply;

            if (darAsset.Any())
            {
                throw new Exception($"Circulating supply exist for {a.DARTicker}");
            }

            a.ProcessID = "1";
            a.IsActive = true;
            a.Reviewed = 1;

            a.CollectedTimeStamp = DateTime.Now;

            a.Operation = "INSERT";
            a.Deleted = 0;

            try
            {
                string publishstatus = OutstandingSupplyTablePublish(a);
                return 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public override bool Delete(DARViewModel i)
        {
            var a = (OutstandingSupplyViewModel)i;
            DateTime updateTime = DateTime.Now.ToUniversalTime();


            a.Operation = "DELETE";
            a.Deleted = 1;

            try
            {
                string publishstatus = OutstandingSupplyTablePublish(a);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<DARViewModel> GetFinalizedOutStandingSupply(DateTime start)
        {
            List<OutstandingSupplyViewModel> l = new List<OutstandingSupplyViewModel>();

            var startDateTime = start == null ? DateTime.Now : start.Date;

            string query = $@"select CONCAT(o.DARAssetID, o.ProcessID, o.LoadTimestamp) as OustandingSupplyID
                            ,o.ProcessID
                            ,a.DARAssetID
                            ,a.DARTicker
                            ,o.OutstandingSupply as OutstandingSupply
                            ,o.Reviewed
                            ,o.CollectedTimeStamp as CreateTime
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.OutstandingSupply o
                             inner join (
                                                select darAssetID,max(CollectedTimeStamp) as CollectedTimeStamp
                                                from {DARApplicationInfo.SingleStoreCatalogInternal}.OutstandingSupply
                                                where DATE(CollectedTimeStamp) =  DATE(@start)
                                                group by darAssetId
                                            ) l on o.darAssetID = l.darAssetId and o.CollectedTimeStamp = l.CollectedTimeStamp
                            inner join {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on o.darAssetID = a.DARAssetID
                            where CAST(o.CollectedTimeStamp as date) > CAST(date_sub(@start, interval 1 day) AS DATE)
                            AND CAST(o.CollectedTimeStamp as DATE) < CAST(@start AS DATE) + 1 and o.deleted = 0";

            var p = new DynamicParameters();
            p.Add("@start", startDateTime);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<OutstandingSupplyViewModel>(query, p).ToList();
            }

            return l;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<OutstandingSupplyViewModel> l = new List<OutstandingSupplyViewModel>();

            string sql = $@"SELECT  CONCAT(o.DARAssetID, o.ProcessID, o.LoadTimestamp) as OustandingSupplyID
                        ,o.ProcessID
	                    ,a.DARAssetID
	                    ,a.DARTicker
	                    ,o.OutstandingSupply as OutstandingSupply
	                    ,o.Reviewed
	                    ,o.CollectedTimeStamp as CreateTime
                        from {DARApplicationInfo.SingleStoreCatalogInternal}.OutstandingSupply o
                        inner join {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on o.darAssetID = a.DARAssetID where o.deleted = 0;
                        ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<OutstandingSupplyViewModel>(sql).ToList();
            }

            return l;
        }

        public DARViewModel GetOldRecord(string key)
        {
            List<OutstandingSupplyViewModel> l = new List<OutstandingSupplyViewModel>();

            string sql = $@"SELECT  CONCAT(o.DARAssetID, o.ProcessID, o.LoadTimestamp) as OustandingSupplyID
                        ,o.ProcessID
	                    ,o.DARAssetID
	                    ,o.OutstandingSupply as OutstandingSupply
	                    ,o.Reviewed
	                    ,o.CollectedTimeStamp as CollectedTimeStamp
	                    ,o.CollectedTimeStamp as LoadTimestamp
                        from {DARApplicationInfo.SingleStoreCatalogInternal}.OutstandingSupply o
                        where  CONCAT(o.DARAssetID, o.ProcessID, o.LoadTimestamp) = '{key}'
                        ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<OutstandingSupplyViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }


        public override DARViewModel Get(string key)
        {
            return Get().Cast<OutstandingSupplyViewModel>().Where(x => x.darAssetID.ToUpper().Equals(key.ToUpper())).FirstOrDefault();
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            var a = (OutstandingSupplyViewModel)i;

            var old_record = GetOldRecord(a.OustandingSupplyID);

            try
            {
                Delete(old_record);
                a.Reviewed = 1;
                Add(a);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public IEnumerable<DARViewModel> Get(DateTime asOfDate)
        {
            List<OutstandingSupplyViewModel> l = new List<OutstandingSupplyViewModel>();

            string sql = $@"

                    select
                        o.ProcessID
                        ,a.DARAssetID
                        ,a.DARTicker
                        ,o.OutstandingSupply as OutstandingSupply
                        ,o.Reviewed
                        ,o.CollectedTimeStamp as CreateTime
                        ,null as MappedDARAssetId
                        ,a.LegacyDARAssetId
                        ,a.LegacyId
                    from (  select darAssetID,LAST(OutstandingSupply,CollectedTimeStamp) as OutstandingSupply, max(ProcessID) as ProcessID,max(Reviewed) as Reviewed, max(CollectedTimeStamp) as CollectedTimeStamp
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.OutstandingSupply
                            where DELETED = 0 AND CollectedTimeStamp <=  @asOfdate
                            group by darAssetID
                            ) o
                    inner join {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on o.darAssetID = a.DARAssetID
               

                ";
            var p = new DynamicParameters();
            p.Add("@Asofdate", asOfDate);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<OutstandingSupplyViewModel>(sql, p).ToList();
            }

            return l;
        }


        public IEnumerable<DARViewModel> GetClientCirculatingSupply(DateTime asOfDate, string clientID)
        {
            

            List<OutstandingSupplyViewModel> l = new List<OutstandingSupplyViewModel>();

            string sql = $@"

                    select
                        o.ProcessID
                        ,a.DARAssetID
                        ,a.DARTicker
                        ,o.OutstandingSupply as OutstandingSupply
                        ,o.Reviewed
                        ,o.CollectedTimeStamp as CreateTime
                        ,null as MappedDARAssetId
                        ,a.LegacyDARAssetId
                        ,a.LegacyId
                    from (  select darAssetID,LAST(OutstandingSupply,CollectedTimeStamp) as OutstandingSupply, max(ProcessID) as ProcessID,max(Reviewed) as Reviewed, max(CollectedTimeStamp) as CollectedTimeStamp
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.OutstandingSupply
                            where DELETED = 0 AND CollectedTimeStamp <=  @asOfdate
                            group by darAssetID
                            ) o
                    inner join {DARApplicationInfo.SingleStoreCatalogInternal}.vClientAssetsClientID a on o.darAssetID = a.DARAssetID
                    where a.CallerID = '{clientID}'

                ";
            var p = new DynamicParameters();
            p.Add("@Asofdate", asOfDate);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<OutstandingSupplyViewModel>(sql, p).ToList();
            }

            return l;
        }

        public override bool IdExists(string nextId)
        {
            throw new NotImplementedException();
        }

        public override string GetNextId()
        {
            throw new NotImplementedException();
        }

        public string OutstandingSupplyTablePublish(OutstandingSupplyViewModel l)
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

            var topic_name = string.Empty;
            if (DARApplicationInfo.CurrentEnvironment.Equals("DEV"))
            {
                topic_name = "outstanding_supply_test_0";
            }
            else
            {
                topic_name = "outstanding_supply_prod_0";
            }

            string jsondata = JsonSerializer.Serialize(l);
            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    producer.Produce(topic_name, new Message<Null, string> { Value = jsondata });
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