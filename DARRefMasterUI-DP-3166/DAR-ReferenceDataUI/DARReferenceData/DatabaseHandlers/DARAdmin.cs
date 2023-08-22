using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class DARAdmin : RefDataHandler
    {
        #region User

        public IEnumerable<DARUserViewModel> GetUsers()
        {
            List<DARUserViewModel> l = new List<DARUserViewModel>();

            string sql = $@"
                            SELECT t.ID as UserId
	                              ,t.UserName as UserName
	                          FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Users t

                          ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<DARUserViewModel>(sql).ToList();
            }

            return l;
        }

        public DARUserViewModel GetUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            return GetUsers().Cast<DARUserViewModel>().Where(x => x.UserName.ToUpper().Equals(userName.ToUpper())).FirstOrDefault();
        }

        public bool DeleteUser(DARUserViewModel a)
        {
            /*
                        string sql = $@"DELETE {DARApplicationInfo.SingleStoreCatalogInternal}.AspNetUsers
                                       WHERE Id = @UserId
                                        ";
            */
            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLUsersDEL(@ID)";
            var p = new DynamicParameters();
            p.Add("@ID", a.UserId);

            string deleteId = string.Empty;

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
                deleteId = (string)result.Values.First();
            }

            return !string.IsNullOrEmpty(deleteId) && deleteId == a.UserId;
        }

        #endregion User

        #region Role

        public IEnumerable<DARRoleViewModel> GetRoles()
        {
            List<DARRoleViewModel> l = new List<DARRoleViewModel>();

            string sql = $@"
                            SELECT t.ID as RoleID
	                              ,t.Name as RoleName
	                          FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Roles t
                              Where Deleted = 0
                          ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<DARRoleViewModel>(sql).ToList();
            }

            return l;
        }

        public DARRoleViewModel GetRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return null;

            return GetRoles().Cast<DARRoleViewModel>().Where(x => x.RoleName.ToUpper().Equals(roleName.ToUpper())).FirstOrDefault();
        }




        public void AddRole(DARRoleViewModel a)
        {
            a.RoleId = GetNextId();
            a.Operation = OPS_INSERT;
            a.Deleted = 0;
            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.IsActive = true;
            Publish(a);
        }

        public bool DeleteRole(DARRoleViewModel a)
        {

            a.Operation = OPS_DELETE;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.Deleted = 1;
            Publish(a);
            return true;
        }


        #endregion

        #region UserRole

        public IEnumerable<DARUserRoleViewModel> GetUserRoles()
        {
            List<DARUserRoleViewModel> l = new List<DARUserRoleViewModel>();

            string sql = $@"
                              SELECT
                                    u.Id + '.' + r.Id as ID
		                            ,u.Id as UserId
		                            ,u.UserName
		                            ,r.Id as RoleId
		                            ,r.Name as RoleName
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Users u
                            INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.UserRoles ur on u.Id = ur.UserId
                            INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Roles r on ur.RoleId = r.Id
                            Where Deleted = 0

                          ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<DARUserRoleViewModel>(sql).ToList();
            }

            return l;
        }

        public bool AddUserRole(DARUserRoleViewModel a)
        {

            if (string.IsNullOrEmpty(a.UserId))
            {
                var u = GetUser(a.UserName);
                if (u == null)
                    throw new Exception("Invalid user");
                a.UserId = u.UserId;
            }

            if (string.IsNullOrEmpty(a.RoleId))
            {
                var r = GetRole(a.RoleName);
                if (r == null)
                    throw new Exception("Invalid role");
                a.RoleId = r.RoleId;
            }

            a.Operation = OPS_INSERT;
            a.Deleted = 0;
            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.IsActive = true;
            Publish(a);

            return true;

        }

        public bool DeleteUserRole(DARUserRoleViewModel i)
        {
            var a = (DARUserRoleViewModel)i;
            a.Operation = OPS_DELETE;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.Deleted = 1;

            Publish(a);
            return true;
        }

        #endregion UserRole

        #region AppModule

        public IEnumerable<AppModuleViewModel> GetAppModules()
        {
            List<AppModuleViewModel> l = new List<AppModuleViewModel>();

            string sql = $@"
                            SELECT DARAppModuleID
                                  , ModuleName
                                  , Description as ModuleDescription
                                  , Link as ModuleLink
                                  , IsActive
                                  , CreateUser
                                  , LastEditUser
                                  , CreateTime
                                  , LastEditTime
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AppModule
                          ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<AppModuleViewModel>(sql).ToList();
            }

            return l;
        }

        public AppModuleViewModel GetAppModules(string moduleName)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
                return null;

            return GetAppModules().Cast<AppModuleViewModel>().Where(x => x.ModuleName.ToUpper().Equals(moduleName.ToUpper())).FirstOrDefault();
        }

        public long AddAppModule(AppModuleViewModel a)
        {
            a.Operation = OPS_INSERT;
            if (string.IsNullOrWhiteSpace(a.DARAppModuleID))
            {
                a.DARAppModuleID = GetNextAppModuleID();
            }
            a.Deleted = 0;
            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.IsActive = true;
            Publish(a);
            return 0;
        }

        public string GetNextAppModuleID()
        {
            return GetNextId("DAM", 5, 3);
        }


        public bool DeleteAppModule(AppModuleViewModel a)
        {
            a.Operation = OPS_DELETE;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.Deleted = 1;

            Publish(a);
            return false;
        }

        public bool UpdateAppModule(AppModuleViewModel a)
        {

            Delete(a);
            Add(a);

            return true;
        }

        #endregion AppModule

        #region AppModuleRole

        public IEnumerable<RoleAppModuleViewModel> GetAppModuleRoles()
        {
            List<RoleAppModuleViewModel> l = new List<RoleAppModuleViewModel>();

            string sql = $@"
                             SELECT
                                 cast(r.ID as char)  + '.' + cast(am.DARAppModuleID as char) as RoleAppModuleId
	                            ,r.Id as RoleId
	                            ,r.Name as RoleName
	                            ,ra.AppModuleId as AppModuleId
	                            ,am.ModuleName
	                            ,am.Description as ModuleDescription
	                            ,am.Link as ModuleLink
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Roles r
                            INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.RoleAppModule ra on ra.RoleId = r.Id
                            INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.AppModule am on am.DARAppModuleID = ra.AppModuleId;
                          ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<RoleAppModuleViewModel>(sql).ToList();
            }

            return l;
        }

        public List<string> GetModuleRoles(string moduleName)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
                return null;

            return GetAppModuleRoles().Cast<RoleAppModuleViewModel>().Where(x => x.ModuleName.ToUpper().Equals(moduleName.ToUpper())).Select(x => x.RoleName).ToList();
        }

        public long AddRoleAppModule(RoleAppModuleViewModel a)
        {
            var r = GetRole(a.RoleName);
            var app = GetAppModules(a.ModuleName);
            a.AppModuleId = app.DARAppModuleID;
            a.RoleId = r.RoleId;
            a.Operation = OPS_INSERT;
            a.Deleted = 0;
            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.IsActive = true;
            Publish(a);
            return 0;
        }

        public bool DeleteAppModuleRole(RoleAppModuleViewModel a)
        {
            a.Operation = OPS_DELETE;
            a.Deleted = 1;
            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.IsActive = true;
            Publish(a); 
            return true;
        }

        #endregion AppModuleRole

        #region Implementation Methods

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
            throw new NotImplementedException();
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
            string sql = string.Empty;

            if (nextId.StartsWith("DR"))
            {
                sql = $@"
                            select ID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Roles
                            where ID = '{nextId}'
                            ";
            }
            else if (nextId.StartsWith("DAM"))
            {
                sql = $@"
                            select DARAppModuleID as ID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.AppModule
                            where DARAppModuleID = '{nextId}'
                            ";
            }
            else
            {
                throw new Exception("GetNextId invalid string");
            }

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var r = connection.Query<string>(sql);

                if (r != null && r.Any())
                    return true;
            }

            return false;
        }

        public override string GetNextId()
        {
            return GetNextId("DR", 5, 3);
        }

        #endregion Implementation Methods

        #region Publish
        public string Publish(DARRoleViewModel l)
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
                    producer.Produce("role", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }

        public string Publish(DARUserRoleViewModel l)
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
                    producer.Produce("user_access", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }

        public string Publish(AppModuleViewModel l)
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
                    producer.Produce("app_modules", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }

        public string Publish(RoleAppModuleViewModel l)
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
                    producer.Produce("app_module_access", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }
        

        #endregion
    }
}