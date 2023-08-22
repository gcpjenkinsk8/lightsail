using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class ExchangeVettingStatusViewModel : DARViewModel
    {
        public long ID { get; set; }

        public string ProcessName { get; set; }
        public long ProcessId { get; set; }

        public string Exchange { get; set; }
        public long ExchangeId { get; set; }

        public string DARExchangeID { get; set; }

        public string VettingStatus { get; set; }
        public long VettingStatusId { get; set; }

        public override string GetDescription()
        {
            return $"{ProcessName} - {Exchange} - {VettingStatus}";
        }
    }
}