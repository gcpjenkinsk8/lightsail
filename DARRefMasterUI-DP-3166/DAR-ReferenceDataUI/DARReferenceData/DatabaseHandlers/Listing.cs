using Dapper;
using DARReferenceData.ViewModels;
using log4net;
using log4net.Repository.Hierarchy;
using MySql.Data.MySqlClient;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class Listing : RefDataHandler
    {
        // TODO
        public override long Add(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        // TODO
        public override bool Delete(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DARViewModel> Get()
        {
            var l = new List<ListingViewModel>();

            string sql = $@"
                           SELECT DARAssetID,DARExchangeID,ExchangeAssetName,ExchangeAssetTicker
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.vListing";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ListingViewModel>(sql).ToList();
            }

            return l;
        }

        public ListingViewModel GetListingById(string DARExchangeId, string identifier)
        {
            if (string.IsNullOrWhiteSpace(DARExchangeId))
                return null;

            string sql = $@"
                            select DARAssetID,DARExchangeID,ExchangeAssetName,ExchangeAssetTicker
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.vListing
                            where DARExchangeID = '{DARExchangeId}'
                              and (ExchangeAssetName = '{identifier}' or ExchangeAssetTicker = '{identifier}')
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                return connection.Query<ListingViewModel>(sql).FirstOrDefault();
            }
        }

        public override DARViewModel Get(string key)
        {
            throw new NotImplementedException();
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        // TODO
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