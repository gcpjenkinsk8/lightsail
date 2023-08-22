using DARReferenceData.DatabaseHandlers;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData
{
    public enum Envorinments
    {
        DEV = 1,
        QA = 2,
        UAT = 3,
        PROD = 4
    }

    public class DARApplicationInfo
    {
        public static string ApplicationName = "RefmasterUI";
        public static string SingleStorePublicDB { get; set; }

        public static string SingleStoreInternalDB { get; set; }
        public static string SingleStoreCatalogInternal { get; set; }
        public static string SingleStoreCatalogPublic { get; set; }

        public static string ReferenceDB { get; set; }
        public static string SingleStoreCatalogMetadata { get; set; }

        public static string SingleStoreCatalogDax { get; set; }

        public static string SingleStoreCatalogDardb { get; set; }

        public static string LogDatabase { get; set; }

        public static string CalcPriceDatabase { get; set; }

        public static string BlockChainDatabase { get; set; }

        public static string ClientReportingDatabase { get; set; }
        public static string CurrentDB { get; set; }

        public static string CurrentUserMessage { get; set; }

        public static string KafkaCertificateFileName { get; set; }

        public static string KafkaServerName { get; set; }

        public static string KafkaSaslUsername { get; set; }
        public static string KafkaPassword { get; set; }


        private static Dictionary<string, List<string>> _darRoles;

        public static Dictionary<string, List<string>> DARRoles
        {
            get
            {
                return _darRoles;
            }
        }

        public static string CurrentEnvironment { get;set; }

        static DARApplicationInfo()
        {
            Init();
        }

        public static void LoadDarRoles()
        {
            if (_darRoles == null)
                _darRoles = new Dictionary<string, List<string>>();

            DARAdmin dhAdmin = new DARAdmin();
            var mr = dhAdmin.GetAppModuleRoles();
            foreach (var m in mr)
            {
                if (_darRoles.ContainsKey(m.ModuleName))
                {
                    _darRoles[m.ModuleName].Add(m.RoleName);
                }
                else
                {
                    _darRoles.Add(m.ModuleName, new List<string>() { m.RoleName });
                }
            }
        }

        private static void Init()
        {
            try
            {
                string environment = ConfigurationManager.AppSettings["Environment"];
                CurrentEnvironment = environment;
                Envorinments e = (Envorinments)Enum.Parse(typeof(Envorinments), environment);
                // ReferenceDataMasterDB = GetInfo();
                InitializeDbAttributes();
                SingleStorePublicDB = GetSingleStorePublicDBInfo();
                SingleStoreInternalDB = GetSingleStoreInternalDBInfo();
                ReferenceDB = GetSingleStoreReferenceDBInfo();
                CurrentUserMessage = "Click to Login";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //LoadDarRoles();
        }

        private static string GetSingleStoreInternalDBInfo()
        {
            try
            {
                string conn;
                conn = ConfigurationManager.AppSettings["SingleStoreInternal"];
                return conn;
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(@"c:\temp\refmaster.txt", true))
                {
                    writer.WriteLine($"Failed to get connection string {ex.Message} - {DateTime.Now}");
                }
                return null;
            }
        }

        private static string GetSingleStoreReferenceDBInfo() {
            try
            {
                string conn;

                conn = ConfigurationManager.AppSettings["ReferenceURL"];
                return conn;
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(@"c:\temp\refmaster.txt", true))
                {
                    writer.WriteLine($"Failed to get connection string {ex.Message} - {DateTime.Now}");
                }
                return null;
            }
        }

        private static string GetSingleStorePublicDBInfo()
        {
            try
            {
                string conn;

                conn = ConfigurationManager.AppSettings["SingleStorePublic"];
                return conn;
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(@"c:\temp\refmaster.txt", true))
                {
                    writer.WriteLine($"Failed to get connection string {ex.Message} - {DateTime.Now}");
                }
                return null;
            }
        }

        private static string InitializeDbAttributes()
        {
            try
            {
                string conn;
                /*
                                if(ConfigurationManager.AppSettings["Environment"].Equals("DEV"))
                                {
                                    using (StreamWriter writer = new StreamWriter(@"c:\temp\refmaster.txt", true))
                                    {
                                        writer.WriteLine($"Retrieved connection string - {DateTime.Now}");
                                        writer.WriteLine($"Current directory is {Directory.GetCurrentDirectory()}");
                                        conn = System.IO.File.ReadAllText(@"c:\store\dataprod.txt");

                                        var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                                        var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
                                        connectionStringsSection.ConnectionStrings["DefaultConnectionStringName"].ConnectionString = conn.Replace("ReferenceCore", "ReferenceCore-Dev");
                                        config.Save();
                                        ConfigurationManager.RefreshSection("connectionStrings");
                                    }
                                }
                                else
                                {
                                }*/
                conn = ConfigurationManager.AppSettings["Connection"];

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Environment"]) && ConfigurationManager.AppSettings["Environment"].ToUpper().Equals("PROD"))
                {
                    CurrentDB = "ReferenceCore";
                    SingleStoreCatalogInternal = "refmaster_internal";
                    SingleStoreCatalogPublic = "refmaster_public";
                    SingleStoreCatalogDax = "dax";
                    SingleStoreCatalogDardb = "dardb";
                    SingleStoreCatalogMetadata = "metadata";
                    CalcPriceDatabase = "calcprice";
                    BlockChainDatabase = "blockchain";
                    KafkaCertificateFileName = @"C:\certificate\caprod.pem";
                    LogDatabase = "logging";
                    ClientReportingDatabase = "clientReporting";
                }
                else
                {
                    SingleStoreCatalogInternal = "refmaster_internal_DEV";
                    SingleStoreCatalogPublic = "refmaster_public_DEV";
                    SingleStoreCatalogDax = "dax_DEV";
                    SingleStoreCatalogDardb = "dardb_DEV";
                    SingleStoreCatalogMetadata = "metadata_DEV";
                    CalcPriceDatabase = "calcprice_DEV";
                    BlockChainDatabase = "blockchain_DEV";
                    KafkaCertificateFileName = @"C:\certificate\canonprod.pem";
                    LogDatabase = "logging_DEV";
                    ClientReportingDatabase = "clientReporting_DEV";

                    CurrentDB = "ReferenceCore-Dev";
                    conn = conn.Replace("ReferenceCore", CurrentDB);
                }

                KafkaServerName = ConfigurationManager.AppSettings["KafkaServerName"];
                KafkaSaslUsername = ConfigurationManager.AppSettings["KafkaSaslUsername"];
                KafkaPassword = ConfigurationManager.AppSettings["KafkaPassword"];



                return conn;
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(@"c:\temp\refmaster.txt", true))
                {
                    writer.WriteLine($"Failed to get connection string {ex.Message} - {DateTime.Now}");
                }
                return null;
            }
        }
    }
}