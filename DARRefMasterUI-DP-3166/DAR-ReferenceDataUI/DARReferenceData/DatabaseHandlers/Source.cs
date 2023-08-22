using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class Source : RefDataHandler
    {
     
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
            IEnumerable<SourceViewModel> r;

            string sql = $@"
                            SELECT *
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.vSource
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            { 
                r = connection.Query<SourceViewModel>(sql);
            }
            return r;
        }

        public override DARViewModel Get(string key)
        {
            SourceViewModel r;

            string sql = $@"
                            SELECT *
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.vSource
                            WHERE ( ShortName = '{key}'  or DARSourceID = '{key}' );
                            ";
            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                r = connection.Query<SourceViewModel>(sql).FirstOrDefault();
            }

            return r;
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