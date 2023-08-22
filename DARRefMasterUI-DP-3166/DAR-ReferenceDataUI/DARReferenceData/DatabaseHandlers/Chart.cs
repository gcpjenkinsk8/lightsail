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
    public class Chart : RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        ObjectCache cache = MemoryCache.Default;

        public Chart()
        {
        }

        public IEnumerable<ChartModelViewsChart> GetNewDerivativeContractByDay()
        {
            List<ChartModelViewsChart> l = new List<ChartModelViewsChart>();

            try
            {
                string sql = $@"
                            select date_format(CreateTme, '%m/%d') as category, count(*) as value
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.DerivativesContractID 
                            where CreateTme  >  DATE_ADD(now(), INTERVAL -7 DAY)
                            group by date_format(CreateTme, '%M/%d')
                            order by date_format(CreateTme, '%M/%d')
                                ";

                using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
                {
                    l = connection.Query<ChartModelViewsChart>(sql).ToList();
                }
            }
            catch (Exception ex)
            {
                //TODO send alert to log topic kafka

            }
            return l;
        }

        public IEnumerable<ChartModelViewsChart> GetApiCallCount()
        {
            List<ChartModelViewsChart> l = new List<ChartModelViewsChart>();

            try
            {
                string sql = $@"
                                    select c.ClientName as category,count(*) as value
                                    from logging.logUsage l 
                                    inner join refmaster_internal.ClientIPs i on l.CallerID = i.CallerID
                                    inner join refmaster_internal.Clients c on i.ClientID = c.ID
                                    where l.calledTS  >  DATE_ADD(now(), INTERVAL -7 DAY)
                                      and c.ClientName not in (select user from dds.api_reporting_filterout)
                                    group by  c.ClientName
                                    order by  c.ClientName
                                ";

                using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
                {
                    l = connection.Query<ChartModelViewsChart>(sql).ToList();
                }
            }
            catch(Exception ex)
            {
                //TODO send alert to log topic kafka
            }
            return l;
        }

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