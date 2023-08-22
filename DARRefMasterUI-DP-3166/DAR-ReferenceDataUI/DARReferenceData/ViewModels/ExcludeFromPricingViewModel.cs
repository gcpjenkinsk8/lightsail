using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DARReferenceData.DatabaseHandlers;

namespace DARReferenceData.ViewModels
{
    public class ExcludeFromPricingViewModel : DARViewModel
    {
        public string ExcludefromPricingID { get; set; }
        public string DARExchangeID { get; set; }
        public string ExchangeName { get; set; }
        public string ExchangePair { get; set; }
        public string Operation { get; set; }

        public long Deleted { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public override string GetDescription()
        {
            return $"{DARExchangeID} - {ExchangePair}";
        }
    }
}
