using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class MarketCapViewModel: DARViewModel
    {
        public string priceIdentifier { get; set; }

        public string darAssetID { get; set; }

        private string _darAssetTicker;

        public string darAssetTicker
        {
            get
            {
                return _darAssetTicker;
            }
            set
            {
                _darAssetTicker = value.ToUpper();

            }
        }

        public decimal marketCap { get; set; }

        public string effectiveTime { get; set; }

        public string error { get; set; }

        public override string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}
