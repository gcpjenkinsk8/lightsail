using Dapper;
using DARReferenceData.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class TokenTable : RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        public override IEnumerable<DARViewModel> Get()
        {
            List<TokenTableViewModel> l = new List<TokenTableViewModel>();

            string sql = $@"select                         
                            ,e.legacyID
                            ,e.darTicker
                            ,e.name
                            ,e.darAssetID
                            ,e.createTime
                            
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.token2 e
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<TokenTableViewModel>(sql).ToList();
            }

            return l;
        }

        public TokenTableViewModel GetTokenDetails(string id)
        {
            List<TokenTableViewModel> l = new List<TokenTableViewModel>();

            string sql = $@"select                         
                            e.legacyID
                            ,upper(e.darTicker) as darTicker
                            ,e.name
                            ,e.darAssetID
                            ,e.createTime
                            
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.token2 e
                            WHERE DARAssetID = '{id}' or darTicker = '{id}'
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<TokenTableViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }

        public TokenTableViewModel GetTokenDetailsName(string id)
        {
            List<TokenTableViewModel> l = new List<TokenTableViewModel>();

            string sql = $@"select                         
                            e.legacyID
                            ,upper(e.darTicker) as darTicker
                            ,e.name
                            ,e.darAssetID
                            ,e.createTime
                            
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.token2 e
                            WHERE name = '{id}'
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<TokenTableViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
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

        public override long Add(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(DARViewModel i)
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
    }
}