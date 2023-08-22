using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class PriceViewModel
    {
        public string DARAssetID { get; set; }
        public string Ticker { get; set; }

        public decimal LastPrice { get; set; }
    }
}
