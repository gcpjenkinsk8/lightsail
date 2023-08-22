using System;
using System.Collections.Generic;
using System.Web;
using Dapper;
using DARReferenceData.ViewModels;
using DARReferenceData.Models;
using MySql.Data.MySqlClient;

namespace DARReferenceData.DatabaseHandlers
{
    public class CallerID : RefDataHandler
    {
        public override long Add(DARViewModel i)
        {
            var user = (CallerIDViewModel)i;
            DateTime ts = DateTime.Now.ToUniversalTime();
            user.CreateTime = user.LastEditTime = ts;
            user.CreateUser = user.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            user.IsActive = true;

            ClientViewModel cv = (ClientViewModel)(new Client()).Get(user.ClientName);

            string cmd = $@"INSERT INTO {DARApplicationInfo.SingleStoreCatalogInternal}.ClientIPs
                        (CallerID, DARClientID, EmailAddress, CreateTime, CreateUser, LastEditTime, LastEditUser)
                        VALUES (@CallerID, @DARClientID, @EmailAddress, @CreateTime, @CreateUser, @LastEditTime, @LastEditUser);
                        SELECT last_insert_id()";

            var parameters = new DynamicParameters();

            Type t = user.GetType();
            var props = t.GetProperties();

            foreach (var prop in props)
            {
                var value = prop.GetValue(user, new object[] { });
                parameters.Add($"@{prop.Name}", value);
            }

            parameters.Add("@DARClientID", cv.DARClientID);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var identity = connection.ExecuteScalar(cmd, parameters);
                //user.ID = long.Parse(identity.ToString());
            }

            //return user.ID;
            return 1;
        }

        public override bool Delete(DARViewModel i)
        {
            var client = (CallerIDViewModel)i;

            string cmd = $"DELETE FROM {DARApplicationInfo.SingleStoreCatalogInternal}.ClientIPs WHERE CallerID='{client.CallerID}'";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                try
                {
                    connection.Execute(cmd);
                }
                catch
                {
                    throw new Exception(MySqlErrorCode.CannotFindSystemRecord.ToString());
                }
            }

            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            IEnumerable<CallerIDViewModel> callers;

            string cmd = $@"SELECT ip.*, ClientName, Description FROM {DARApplicationInfo.SingleStoreCatalogInternal}.ClientIPs ip
                        INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Clients c ON c.DARClientID = ip.DARClientID ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                callers = connection.Query<CallerIDViewModel>(cmd);
            }
            return callers;
        }

        public override DARViewModel Get(string key)
        {
            throw new NotImplementedException();
        }

        public override string GetNextId()
        {
            throw new NotImplementedException();
        }

        public override bool IdExists(string nextId)
        {
            throw new NotImplementedException();
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            var client = (CallerIDViewModel)i;
            DateTime ts = DateTime.Now.ToUniversalTime();
            client.LastEditTime = ts;
            client.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            string cmd = $@"UPDATE {DARApplicationInfo.SingleStoreCatalogInternal}.ClientIPs SET
                        EmailAddress=@EmailAddress,
                        LastEditTime=@LastEditTime,
                        LastEditUser=@LastEditUser
                        WHERE ID=@ID;";

            var parameters = new DynamicParameters();

            Type t = client.GetType();
            var props = t.GetProperties();

            foreach (var prop in props)
            {
                var value = prop.GetValue(client, new object[] { });
                parameters.Add($"@{prop.Name}", value);
            }

            int rows = 0;

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                rows = connection.Execute(cmd, parameters);
            }

            return rows > 0;
        }
    }
}