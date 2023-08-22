using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DARReferenceData.DatabaseHandlers
{
    public enum LogDataType
    {
        Error
        ,Info
        ,Warning
    }
    public class LogData : RefDataHandler
    {

        public static void Log(LogDataType message_type, string message, Exception ex, bool log_inner_exception = false)
        {
            Log(message_type, $"{message}. ERROR:[{ex.Message}]. INNER EXCEPTION:{ex.ToString()}");
        }

        public static void Log(LogDataType message_type, string message)
        {
            LogViewModel l = new LogViewModel()
            {
                ApplicationName = DARApplicationInfo.ApplicationName,
                MessageType = message_type.ToString(),
                Message = message
            };

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
                    producer.Produce("refmaster_logs", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
            }
            catch (Exception ex)
            {

            }

        }

  

        public override long Add(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<LogViewModel> l = new List<LogViewModel>();

            string sql = $@"

                        SELECT Date as CreateTime
                                ,Message
                          FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Log4NetLog

                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<LogViewModel>(sql).ToList();
            }

            return l;
        }

        public IEnumerable<DARViewModel> GetLogByType(string logType, string minutes)
        {

            List<LogViewModel> l = new List<LogViewModel>();

            string sql = $@"

                        SELECT *
                          FROM {DARApplicationInfo.LogDatabase}.refmasterLogs
                          WHERE ApplicationName = 'RefmasterUI'
                            and loadTime > DATE_ADD(now(), INTERVAL -{minutes} MINUTE) 
                          order by loadTime desc
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<LogViewModel>(sql).ToList();
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