using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class ExchangeStatusViewModel : DARViewModel
    {
        public string ExchangeStatusID { get; set; }
        public string DARExchangeID { get; set; }
        public string DARMnemonicFamily { get; set; }

        public long vettedStatus { get; set; }

        public string Operation { get; set; }

        public long Deleted { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public decimal LoadTimestamp { get; set; }
        public override string GetDescription()
        {
            return $"{DARExchangeID} - {DARMnemonicFamily} - {vettedStatus}";
        }
    }
}