using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.DatabaseHandlers
{
    public class EventType : RefDataHandler
    {
        public IList<EventTypeViewModel> GetAssets()
        {
            List<EventTypeViewModel> l = new List<EventTypeViewModel>();

            string sql = $"select * from {DARApplicationInfo.SingleStoreCatalogInternal}.Event";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<EventTypeViewModel>(sql).ToList();
            }

            return l;
        }

        public EventTypeViewModel GetAssetByEventName(string darEventName)
        {
            if (string.IsNullOrWhiteSpace(darEventName))
                return null;

            EventTypeViewModel l = new EventTypeViewModel();
            string sql = $@"SELECT DAREventID  as DAREventTypeID
                            ,EventName
                            , CreateUser
                            , CreateTime
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Event
                            where EventName = '{darEventName}'";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<EventTypeViewModel>(sql).FirstOrDefault();
            }

            return l;
        }

        public override long Add(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
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