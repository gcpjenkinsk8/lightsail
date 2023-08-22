using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using Dapper;
using DARReferenceData.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using Spire.Xls;
using FluentValidation;
using DARReferenceData.DatabaseHandlers.Validators;
using FluentValidation.Results;
using System.ComponentModel;
using System.Runtime.Caching;

namespace DARReferenceData.DatabaseHandlers
{
    public class ServList : RefDataHandler
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
            var l = new List<ServListViewModel>();

            string sql = $@"
                        SELECT *
                        FROM {DARApplicationInfo.SingleStoreCatalogPublic}.serv_list ";

  
            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ServListViewModel>(sql).ToList();
            }

            return l;

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
            throw new NotImplementedException();
        }
    }
}
