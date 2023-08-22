using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.TeleTrust;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class CryptoEvent : RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        private Asset a = new Asset();
        private EventType et = new EventType();

        public static int GetFutureEventCount()
        {
            string sql = $@"
                           select count(*)
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.EventInformation
                            where  EventDate >= timestamp(current_date()) and coalesce(DELETED, 0) = 0
                           ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                return connection.Query<int>(sql).FirstOrDefault();
            }
        }

        public static int GetPastEventCount()
        {
            string sql = $@"
                           select count(*)
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.EventInformation
                            where  EventDate < timestamp(current_date()) and coalesce(DELETED, 0) = 0
                           ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                return connection.Query<int>(sql).FirstOrDefault();
            }
        }

        public StagedCryptoEventViewModel GetolderPublished(string key)
        {
            StagedCryptoEventViewModel l = null;

            string sql = $@"select *
                from {DARApplicationInfo.SingleStoreCatalogInternal}.EventInformation where DAREventID = '{key}'
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<StagedCryptoEventViewModel>(sql).ToList().FirstOrDefault();
            }

            return l;
        }

        public IList<StagedCryptoEventViewModel> GetFinalizedCryptoEvents(string startDate, string endDate)
        {
            List<StagedCryptoEventViewModel> l = new List<StagedCryptoEventViewModel>();

            if (string.IsNullOrWhiteSpace(startDate))
                startDate = DateTime.Today.ToShortDateString();
            if (string.IsNullOrWhiteSpace(endDate))
                endDate = DateTime.Today.ToShortDateString();

            startDate = DARTools.FormatDate_yyyymmdd(startDate);
            endDate = DARTools.FormatDate_yyyymmdd(endDate);

            string sql = $@"select *
                           from {DARApplicationInfo.SingleStoreCatalogInternal}.EventInformation
                            where EventDate >= TIMESTAMP(CAST(@startDate AS DATE))
                              and EventDate < TIMESTAMP(CAST(@endDate AS DATE)+1) and COALESCE(DELETED, 0) = 0;
                            ";

            var p = new DynamicParameters();
            p.Add("@startDate", startDate);
            p.Add("@endDate", endDate);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<StagedCryptoEventViewModel>(sql, p).ToList();
            }

            return l;
        }


        private bool IsInputValid(ref StagedCryptoEventViewModel input)
        {
            StringBuilder error_list = new StringBuilder();
            // Check for duplicate events
            string key = $"{input.DARAssetID.Trim()}{input.EventType.Trim()}{input.EventDate.ToString("yyyy-MM-dd").Trim()}{input.EventDescription.Trim()}".Replace("'", "''");
            var events = GetStagedEvent(key);
            if(events != null)
            {
                throw new Exception($"Event exists!");
            }
            return true;
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            errors = string.Empty;
            StringBuilder sbError = new StringBuilder();
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(fileName);
            Worksheet sheet = workbook.Worksheets[0];

            int rowCount = 0;
            int failCount = 0;
            int successCount = 0;
            int total_rows_to_upload = sheet.Rows.Count() - 1;

            StagedCryptoEventViewModel ce;
            DateTime dt;

            LogData.Log(LogDataType.Info,$"Loading Filename {fileName} RowCount:{total_rows_to_upload}");

            foreach (var row in sheet.Rows)
            {
                if (rowCount == 0)
                {
                    rowCount++;
                    continue;
                }
                ce = new StagedCryptoEventViewModel();
                try
                {
                    if (DateTime.TryParse(row.Columns[0].Value, out dt))
                    {
                        ce.DateofReview = dt;
                    }
                    else
                    {
                        ce.DateofReview = DateTime.Today;
                    }
                    ce.ExchangeAssetTicker = row.Columns[1].Value.Trim();
                    ce.ExchangeAssetName = row.Columns[2].Value.Trim();
                    ce.DARAssetID = row.Columns[3].Value.Trim();

                    Asset asset = new Asset(ce.DARAssetID);
                    if(asset.CurrentAsset  == null)
                    {
                        asset.CurrentAsset = (AssetViewModel)asset.Get(ce.ExchangeAssetTicker);
                        if(asset.CurrentAsset == null)
                        {
                            asset.CurrentAsset = (AssetViewModel)asset.Get(ce.ExchangeAssetName);
                        }
                    }

                    if (asset.CurrentAsset == null)
                    {
                        throw new Exception($"Asset lookup failed: Input Ticker:{ce.ExchangeAssetTicker} Name:{ce.ExchangeAssetName} DARAssetID:{ce.DARAssetID}");
                    }
                    else
                    {
                        ce.DARAssetID = asset.CurrentAsset.DARAssetID;
                        ce.ExchangeAssetTicker = asset.CurrentAsset.DARTicker;
                        ce.ExchangeAssetName = asset.CurrentAsset.Name;
                    }


                    //ce.DAREventID = row.Columns[4].Value;
                    ce.EventType = row.Columns[4].Value;
                    if (DateTime.TryParse(row.Columns[5].Value, out dt))
                    {
                        ce.EventDate = dt;
                    }
                    if (DateTime.TryParse(row.Columns[6].Value, out dt))
                    {
                        ce.AnnouncementDate = dt;
                    }
                    else
                    {
                        ce.AnnouncementDate = ce.EventDate;
                    }

                    ce.EventDescription = Regex.Replace(row.Columns[7].Value, @"\t|\n|\r", ""); 
                    ce.SourceURL = Regex.Replace(row.Columns[8].Value, @"\t|\n|\r", ""); 
                    ce.EventStatus = Regex.Replace(row.Columns[9].Value, @"\t|\n|\r", ""); 
                    ce.Notes = row.Columns[10].Value.Trim();
                    try
                    {
                        ce.Deleted = long.Parse(row.Columns[11].Value);
                    }
                    catch (Exception ex)
                    { 
                        ce.Deleted = 0;
                    }
                    
                    ce.Exchange = row.Columns[12].Value;
                    
                    if (IsInputValid(ref ce))
                       Add(ce);

                    successCount++;
                    
                }
                catch (Exception ex)
                {

                    
                    LogData.Log(LogDataType.Error,$"Failed to load Row[{rowCount}] ERROR:[{ex.Message}] Input:[{ce.GetDescription()}] ");
                    sbError.AppendLine($"Failed to load row {rowCount} :  {ce.GetDescription()} ERROR:{ex.Message}.");
                    failCount++;
                }
                finally
                {
                    if(rowCount % 100 == 0)
                    {
                        LogData.Log(LogDataType.Info, $"Processed Row[{rowCount} of {total_rows_to_upload}]");
                    }
                    rowCount++;
                }
            }

            LogData.Log( LogDataType.Info, $"File:{fileName} Total Rows: {total_rows_to_upload} Success:{successCount} Failed:{failCount}");

            if (sbError.Length != 0)
            {
                errors = sbError.ToString();
                return false;
            }
            else
                return true;
        }

        public bool LogErrorCryptoStagedEvents(StagedCryptoEventViewModel e)
        {
            List<StagedCryptoEventViewModel> l = new List<StagedCryptoEventViewModel>();

            Add(e);

            

            return true;
        }

        #region Published

        public string PublishSelectedEvents(string[] staged_event_ids)
        {
            string result = $"Total { staged_event_ids.Length} events published ";

            int publish_count = 0;
            int error_count = 0;

            LogData.Log(LogDataType.Info, $"[PublishEvent]- Total events to publish: {staged_event_ids.Length}");
            foreach (var id in staged_event_ids)
            {
                var e = GetStagedEvent(id);
                try
                {
                    if (e == null)
                    {
                        LogData.Log(LogDataType.Error, $"[PublishEvent] No data found in staged crypto event table for id: {id}");
                        error_count++;
                    }

                    
                    string errors;
                    if (AddCryptoEventFinal(e, out errors))
                    {
                        if(!Delete(e))
                        {
                            errors = $"[PublishEvent] Failed to delete event with id {id} from staged crypto event table";
                            LogData.Log(LogDataType.Error, errors);
                            e.Error = errors;
                            LogErrorCryptoStagedEvents(e);
                        }
                        publish_count++;
                    }
                    else
                    {
                        e.Error = errors;
                        LogErrorCryptoStagedEvents(e);
                        LogData.Log(LogDataType.Error, errors);
                        error_count++;
                    }
                }
                catch (Exception ex)
                {
                    LogData.Log(LogDataType.Error, $"[PublishEvent] Failed to publish crypto event {e.GetDescription()}", ex);
                    e.Error = ex.Message;
                    LogErrorCryptoStagedEvents(e);
                    error_count++;
                }
            }

            result = $"Total events to publish:{staged_event_ids.Length} Publish count:{publish_count} Error count:{error_count}";
            LogData.Log(LogDataType.Info, $"[PublishEvent]- {result}");
            

            return result;
        }

        public bool PublishStagedCryptoEvents()
        {
            var stagedEvents = Get();

            foreach (var e in stagedEvents)
            {
                try
                {
                    PublishSelectedEvents(new string[] { ((StagedCryptoEventViewModel)e).StageEventID.ToString() });
                }
                catch (Exception ex)
                {
                    LogData.Log(LogDataType.Error, $"Failed to publish crypto event {((StagedCryptoEventViewModel)e).GetDescription()}", ex);
                }
            }

            var remainingEvents = Get();
            if(remainingEvents.Any())
            {
                return false;
            }

            return true;
        }

        public bool AddCryptoEventFinal(StagedCryptoEventViewModel m, out string errors)
        {
            errors = string.Empty;
            StringBuilder sb = new StringBuilder();

            try
            {
                List<StagedCryptoEventViewModel> l = new List<StagedCryptoEventViewModel>();
                DateTime updateTime = DateTime.Now.ToUniversalTime();
                // Check if EventExists already 
                string event_key = $"{m.DARAssetID}{m.EventType}{m.EventDate.ToString("yyyy-MM-dd")}{m.EventDescription}".Replace("'","''");
                var existingEvent = GetPublishedEvent(event_key);
                if (existingEvent != null)
                {
                    sb.AppendLine($"Event {m.GetDescription()} has been published already");
                 
                }


                AssetViewModel avm = (AssetViewModel)a.Get(m.DARAssetID);
                if (avm == null)
                {
                    sb.AppendLine($"Invalid asset id {m.DARAssetID}. Asset doesn't exist in DB");
                }

                EventTypeViewModel etvm = et.GetAssetByEventName(m.EventType);
                if (etvm == null)
                {
                    sb.AppendLine($"Invalid EventType {m.EventType}. Event Type  doesn't exist in DB");
                }
                if (m.EventDescription == null || m.SourceURL == null)
                {
                    sb.AppendLine($"Event {m.GetDescription()}.Event Description and SourceUrl can't be blank");
                }

                if (sb.Length > 0)
                {
                    errors = sb.ToString();
                    return false;
                }

                if (string.IsNullOrEmpty(m.DAREventID))
                {
                    m.DAREventID = GetNextId();
                }
                m.EventTypeID = etvm.DAREventTypeID;
                m.DARAssetID = avm.DARAssetID;
                m.SourceID = "0";
                m.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
                m.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
                m.EventTypeID = etvm.DAREventTypeID;
                m.EventType = etvm.EventName;
                m.CreateTime = updateTime;

                m.Operation = "INSERT";
                m.Deleted = 0;

                string publishstatus = FinalEventsPublish(m);

                return true;

                
            }
            catch (Exception ex)
            {
                sb.AppendLine($"ERROR:{ex.Message} Event {m.GetDescription()}");
                errors = sb.ToString();
                return false;
            }
            
        }

        public bool UpdateCryptoEventFinal(StagedCryptoEventViewModel i)
        {
            List<StagedCryptoEventViewModel> l = new List<StagedCryptoEventViewModel>();
            DateTime updateTime = DateTime.Now.ToUniversalTime();
            i.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            string error;
            AddCryptoEventFinal(i, out error);

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);

            }
                        
            return true;
        }

        public bool DeleteCryptoEventFinal(DARViewModel input)
        {
            StagedCryptoEventViewModel i = (StagedCryptoEventViewModel)input;

            i.Operation = "DELETE";
            i.Deleted = 1;

            string publishstatus = FinalEventsPublish(i);

            
            return true;
            
        }

        #endregion Published

        #region Stage

        

        public void AddStagedCryptoEvent(DARViewModel input)
        {
            StagedCryptoEventViewModel l = (StagedCryptoEventViewModel)input;
            if (!IsInputValid(ref l))
                throw new Exception("Staged Crypto Event exists already");

            Add(input);
        }

        public override long Add(DARViewModel input)
        {
            StagedCryptoEventViewModel i = (StagedCryptoEventViewModel)input;

            DateTime updateTime = DateTime.Now.ToUniversalTime();
            i.ValidationTime = updateTime;

            if (string.IsNullOrWhiteSpace(i.StageEventID))
                i.StageEventID = "0";

            i.LastEditUser = string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.User.Identity.Name) ? Environment.UserName : System.Web.HttpContext.Current.User.Identity.Name;
            i.CreateUser = string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.User.Identity.Name) ? Environment.UserName : System.Web.HttpContext.Current.User.Identity.Name;
            i.Operation = "INSERT";
            i.Deleted = 0;

            string publishstatus = StagedCryptoEventsPublish(i);
            
            return 1;
        }

        public override bool Update(DARViewModel input)
        {
            StagedCryptoEventViewModel i = (StagedCryptoEventViewModel)input;

            List<StagedCryptoEventViewModel> l = new List<StagedCryptoEventViewModel>();
            i.ValidationTime = DateTime.Now.ToUniversalTime();

            var old_record = GetStagedEvent(i.StageEventID);

            if (old_record != null)
            {
                Delete(old_record);
            }
            Add(i);

          
            return true;
        }

        public override bool Delete(DARViewModel input)
        {
            StagedCryptoEventViewModel i = (StagedCryptoEventViewModel)input;
            var old_record = GetStagedEvent(i.StageEventID);

            old_record.LastEditUser = string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.User.Identity.Name) ? Environment.UserName : System.Web.HttpContext.Current.User.Identity.Name;
            old_record.Operation = "DELETE";
            old_record.Deleted = 1;

            string publishstatus = StagedCryptoEventsPublish(old_record);


           
            return true;
            
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<StagedCryptoEventViewModel> l = new List<StagedCryptoEventViewModel>();

            string sql = $@"select (concat(DARAssetID,EventType,DATE_FORMAT(EventDate, '%Y-%m-%d'),LTRIM(RTRIM(EventDescription)))) as  `StageEventID`
                              ,`DateofReview` 
                              ,`ExchangeAssetTicker`
                              ,`ExchangeAssetName`
                              ,`DARAssetID` 
                              ,`EventType`
                              ,`EventDate`
                              ,`AnnouncementDate` 
                              ,`EventDescription`
                              ,`SourceURL`
                              ,`EventStatus`
                              ,`Notes`
                              ,`Deleted`
                              ,`Exchange`
                              ,`ValidationTime` 
                              ,`DARAssetID`
                              ,`DARSourceID` 
                              ,`EventTypeID` 
                              ,`CreateTime`
                              ,`BlockHeight`
                              ,`Error`
                from {DARApplicationInfo.SingleStoreCatalogInternal}.Staging_CryptoNodeEvents where deleted = 0";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<StagedCryptoEventViewModel>(sql).ToList();
            }

            return l;
        }


    
        public StagedCryptoEventViewModel GetStagedEvent(string key)
        {
           StagedCryptoEventViewModel l = null;
            var par = new DynamicParameters();
            par.Add("@key", key.ToString());
            

            string sql = $@"select (concat(DARAssetID,EventType,DATE_FORMAT(EventDate, '%Y-%m-%d'),LTRIM(RTRIM(EventDescription)))) as  `StageEventID` 
                              ,`DateofReview` 
                              ,`ExchangeAssetTicker`
                              ,`ExchangeAssetName`
                              ,`DARAssetID` 
                              ,`EventType`
                              ,`EventDate`
                              ,`AnnouncementDate` 
                              ,`EventDescription`
                              ,`SourceURL`
                              ,`EventStatus`
                              ,`Notes`
                              ,`Deleted`
                              ,`Exchange`
                              ,`ValidationTime` 
                              ,`DARSourceID` 
                              ,`EventTypeID` 
                              ,`CreateTime`
                              ,`BlockHeight`
                              ,`Error`
                from {DARApplicationInfo.SingleStoreCatalogInternal}.Staging_CryptoNodeEvents 
                where deleted = 0 and (concat(LTRIM(RTRIM(DARAssetID)),LTRIM(RTRIM(EventType)),LTRIM(RTRIM(DATE_FORMAT(EventDate, '%Y-%m-%d'))),LTRIM(RTRIM(EventDescription))) = @key)
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<StagedCryptoEventViewModel>(sql, par).ToList().FirstOrDefault();
            }

            return l;
        }
        #endregion Stage
        public StagedCryptoEventViewModel GetPublishedEvent(string key)
        {
            StagedCryptoEventViewModel l = null;
            var par = new DynamicParameters();
            par.Add("@key", key.ToString());


            string sql = $@"select (concat(DARAssetID,EventType,DATE_FORMAT(EventDate, '%Y-%m-%d'),LTRIM(RTRIM(EventDescription)))) as  `StageEventID` 
                              ,`DateofReview`
                              ,`DAREventID`
                              ,`ExchangeAssetTicker`
                              ,`ExchangeAssetName`
                              ,`DARAssetID` 
                              ,`EventType`
                              ,`EventDate`
                              ,`AnnouncementDate` 
                              ,`EventDescription`
                              ,`SourceURL`
                              ,`EventStatus`
                              ,`Notes`
                              ,`Deleted`
                              ,`Exchange`
                              ,`SourceID` 
                              ,`EventTypeID` 
                              ,`CreateTime`
                              ,`BlockHeight`
                from {DARApplicationInfo.SingleStoreCatalogInternal}.EventInformation 
                where deleted = 0 and (concat(LTRIM(RTRIM(DARAssetID)),LTRIM(RTRIM(EventType)),LTRIM(RTRIM(DATE_FORMAT(EventDate, '%Y-%m-%d'))),LTRIM(RTRIM(EventDescription))) = @key)
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<StagedCryptoEventViewModel>(sql, par).ToList().FirstOrDefault();
            }

            return l;
        }
        public override DARViewModel Get(string key)
        {
            StagedCryptoEventViewModel l;
            string sql = $@"select *
                           from {DARApplicationInfo.SingleStoreCatalogInternal}.EventInformation
                            where (concat(DARAssetID,EventType,DATE_FORMAT(EventDate, '%Y-%m-%d'),EventDescription) = '{key}' or DAREventID = '{key}');
                            ";

            
            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<StagedCryptoEventViewModel>(sql).ToList().FirstOrDefault();
            }

            return l;

        }

        public override bool IdExists(string nextId)
        {
            string sql = $@"
                            select DAREventID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.EventInformation
                            where DAREventID = '{nextId}'
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
            return GetNextId("EV", 10, 100);
        }

        public string StagedCryptoEventsPublish(StagedCryptoEventViewModel l)
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
                    producer.Produce("staging_crypto_node_events", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }

        public string FinalEventsPublish(StagedCryptoEventViewModel l)
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
                    producer.Produce("event_information", new Message<Null, string> { Value = jsondata });
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