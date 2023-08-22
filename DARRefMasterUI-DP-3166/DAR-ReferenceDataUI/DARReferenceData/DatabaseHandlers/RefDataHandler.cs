using Dapper;
using DARReferenceData.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DARReferenceData.DatabaseHandlers
{
    public abstract class RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        public const string UT_PRIMARY = "Primary URL";
        public const string UT_TWITTER = "Twitter";
        public const string UT_REDDIT = "Reddit";
        public const string UT_BLOG = "Blog";
        public const string UT_WHITE_PAPER = "White Paper";
        public const string UT_COIN_GECKO_URL = "CoinGecko URL";
        public const string UT_COIN_MARKETCAP_URL = "CoinMarketCap URL";
        public const string UT_CODE_REPOSITORY_URL = "Code Repository URL";
        public const string UT_LINK_ON_DAR_SITE = "Link on DAR Site";
        public const string UT_B_DATA_POINTS_FORM = "B Data Points - Form";
        public const string UT_DAR_THEME = "DAR Themes";
        public const string UT_DATS_THEME = "DATS Themes";


        public const string OPS_INSERT = "INSERT";
        public const string OPS_UPDATE = "UPDATE";
        public const string OPS_DELETE = "DELETE";

        //public static string CatalogName { get; set; }
        //public static string SchemaName { get; set; }

        public RefDataHandler()
        {
            //CatalogName = DARApplicationInfo.CurrentDB;
            //SchemaName = "dbo";
        }

        public string GetRandomAlphanumericString(int length)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                Random r = new Random();
                string nextNumber = r.Next(int.MinValue, int.MaxValue).ToString();
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(nextNumber);
                string s = Convert.ToBase64String(md5.ComputeHash(inputBytes));

                string replacestr = Regex.Replace(s, "[^a-zA-Z0-9_]+", "");

                if (replacestr.Length < length)
                {
                    throw new Exception("Invalid string length");
                }

                int start = r.Next(replacestr.Length - length);

                return $"{replacestr.Substring(start, length).ToUpper()}";
            }
        }

        public string GetNextId(string prefix, int length, int retryCount)
        {
            int maxRetry = retryCount;
            string error;
            try
            {
                for (int i = 0; i < maxRetry; i++)
                {
                    string nextId = $"{prefix}{GetRandomAlphanumericString(length)}";
                    if (!IdExists(nextId))
                        return nextId;

                    if (i > 2)
                    {
                        error = $"{nextId} exists retry {i} of {maxRetry}";
                        Console.WriteLine(error);
                        Logger.Error(error);
                    }
                }

                throw new Exception("Failed to generate ID");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        public abstract bool IdExists(string nextId);

        public abstract string GetNextId();

        public abstract long Add(DARViewModel i);

        public abstract bool Update(DARViewModel i);

        public abstract bool Delete(DARViewModel i);

        public abstract IEnumerable<DARViewModel> Get();

        public abstract DARViewModel Get(string key);

        public abstract bool LoadDataFromExcelFile(string fileName, out string errors);
    }
}