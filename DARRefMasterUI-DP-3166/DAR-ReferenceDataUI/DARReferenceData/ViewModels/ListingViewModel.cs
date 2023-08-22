using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class ListingViewModel : DARViewModel
    {
        public string DARAssetID { get; set; }

        public string DARExchangeID { get; set; }

        public string ExchangeAssetName { get; set; }

        public string ExchangeAssetTicker { get; set; }


        public override string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}
