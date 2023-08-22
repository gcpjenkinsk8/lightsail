using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class ProcessDar : RefDataHandler
    {
        public ProcessViewModel CurrentProcess { get; set; }

        public ProcessDar(string processName)
        {
            if (!string.IsNullOrEmpty(processName))
            {
                Get(processName);
            }
        }

        public ProcessDar()
        {
        }

        public string AddProcess(DARViewModel i)
        {
            var a = (ProcessViewModel)i;

            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLProcess
                        ('UPSERT'
                        , @Name
                        , @Description
                        , @ID
                        , @Lookback
                        , @Frequency
                        , @LookbackUnit
                        , @FrequencyUnit
                        , @IsActive
                        , @CreateUser
                        , @LastEditUser)";

            var p = new DynamicParameters();

            foreach (var prop in a.GetType().GetProperties())
            {
                object value = prop.GetValue(a, new object[] { });
                p.Add($"@{prop.Name}", value);
            }

            long iid = 0;

            try
            {
                //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
                //{
                //    var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
                //    iid = (long)result.Values.FirstOrDefault();

                //    if (iid > 0) a.ID = iid;
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return a.DARProcessID;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (ProcessViewModel)i;

            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLProcess
                        ('DELETE'
                        , @Name
                        , @Description
                        , @ID
                        , @Lookback
                        , @Frequency
                        , @LookbackUnit
                        , @FrequencyUnit
                        , @IsActive
                        , @CreateUser
                        , @LastEditUser
                        )";

            var p = new DynamicParameters();

            foreach (var prop in a.GetType().GetProperties())
            {
                object value = prop.GetValue(a, new object[] { });
                p.Add($"@{prop.Name}", value);
            }

            long deleteId = 0;

            //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //{
            //    var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
            //    deleteId = (long)result.Values.FirstOrDefault();
            //}

            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            var l = new List<ProcessViewModel>();

            string sql = $@"
                            SELECT ID
                                    , Name
                                    , Description
                                    , Lookback
                                    , LookbackUnit
                                    , Frequency
                                    , FrequencyUnit
                                    , IsActive
                                    , CreateUser
                                    , LastEditUser
                                    , CreateTime
                                    , LastEditTime
                                FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Process
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ProcessViewModel>(sql).ToList();
            }

            return l;
        }

        public override DARViewModel Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            CurrentProcess = Get().Cast<ProcessViewModel>().Where(x => x.Name.ToUpper().Equals(key.ToUpper())).FirstOrDefault();
            return CurrentProcess;
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            var a = (ProcessViewModel)i;
            //a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            ///*
            //            string sql = $@"UPDATE [{CatalogName}].{SchemaName}.[Process]
            //                               SET [Name] = @Name
            //                                  ,[Description] = @Description
            //                                  ,[Lookback] = @Lookback
            //                                  ,[LookbackUnit] = @LookbackUnit
            //                                  ,[Frequency] = @Frequency
            //                                  ,[FrequencyUnit] = @FrequencyUnit
            //                                  ,[IsActive] = @IsActive
            //                                  ,[LastEditUser] = @LastEditUser
            //                                  ,[LastEditTime] = @LastEditTime
            //                             WHERE ID = @ID
            //                        ";
            //*/

            //string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLProcess
            //            ('UPSERT'
            //            , @Name
            //            , @Description
            //            , @ID
            //            , @Lookback
            //            , @Frequency
            //            , @LookbackUnit
            //            , @FrequencyUnit
            //            , @IsActive
            //            , @CreateUser
            //            , @LastEditUser)";

            //Type t = a.GetType();
            //PropertyInfo[] props = t.GetProperties();
            //var p = new DynamicParameters();
            //foreach (var prop in props)
            //{
            //    object value = prop.GetValue(a, new object[] { });
            //    p.Add($"@{prop.Name}", value);
            //}

            //long recordId = 0;

            //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //{
            //    var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
            //    recordId = (long)result.Values.FirstOrDefault();
            //}

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

        public override long Add(DARViewModel i)
        {
            throw new NotImplementedException();
        }
    }
}