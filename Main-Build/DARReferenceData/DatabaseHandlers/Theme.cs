using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class Theme : RefDataHandler
    {
        public const string THEME_TYPE_DAR = "DAR";
        public const string THEME_TYPE_DATS = "DATS";

        public ThemeViewModel CurrentTheme { get; set; }

        public static List<DropDownItem> GetThemeTypes()
        {
            var list = new List<DropDownItem>();
            list.Add(new DropDownItem() { Id = "1", Name = THEME_TYPE_DAR });
            list.Add(new DropDownItem() { Id = "2", Name = THEME_TYPE_DATS });
            return list;
        }

        public Theme()
        {
        }

  

        public Theme(string code, string themeType)
        {
            if (!string.IsNullOrWhiteSpace(code))
                Get(code, themeType);
        }

        public override long Add(DARViewModel i)
        {
            var a = (ThemeViewModel)i;

            a.IsActive = true;
            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            a.Operation = "INSERT";
            a.Deleted = 0;

            if (string.IsNullOrEmpty(a.DARThemeID))
            {
                a.DARThemeID = GetNextId();
            }

            if (!string.IsNullOrEmpty(a.ThemeType))
            {
                string publishstatus = ThemePublish(a);
                return 1;
            }
            else
            {
                throw new Exception("Theme type cannot be null");
            }
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (ThemeViewModel)i;
   
            a.Operation = "DELETE";
            a.Deleted = 1;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            string publishstatus = ThemePublish(a);

          
            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<ThemeViewModel> l = new List<ThemeViewModel>();

            string sql = $@"
                            SELECT DARThemeID
                                  , Name
                                  , IsActive
                                  , CreateUser
                                  , LastEditUser
                                  , CreateTime
                                  , LastEditTime
                                  , Description
                                  , ThemeType
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Theme where DELETED = 0";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ThemeViewModel>(sql).ToList();
            }

            return l;
        }

        public DARViewModel Get(string key, string themeType)
        {
            CurrentTheme = Get().Cast<ThemeViewModel>().Where(x => x.Name.ToLower().Equals(key.ToLower()) && x.ThemeType.ToLower().Equals(themeType.ToLower())).FirstOrDefault();
            return CurrentTheme;
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            var a = (ThemeViewModel)i;

            //Delete(a);
            Add(a);


            return true;
        }

        public override DARViewModel Get(string key)
        {
            CurrentTheme = Get().Cast<ThemeViewModel>().Where(x => x.Name.ToLower().Equals(key.ToLower())).FirstOrDefault();
            return CurrentTheme;
        }

        public override bool IdExists(string nextId)
        {
            string sql = $@"
                            select DARThemeID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Theme
                            where DARThemeID = '{nextId}'
                            
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
            return GetNextId("DTHM", 6, 100);
        }

        public string ThemePublish(ThemeViewModel l)
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
                    producer.Produce("theme", new Message<Null, string> { Value = jsondata });
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