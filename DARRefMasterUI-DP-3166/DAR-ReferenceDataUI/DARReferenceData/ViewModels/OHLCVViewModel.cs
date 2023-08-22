using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;

namespace DARReferenceData.ViewModels
{
    public class OHLCVViewModel : DARViewModel
    {
        public string priceIdentifier { get; set; }

        public string darAssetID { get; set; }

        private string _darTicker;

        [JsonProperty("darAssetTicker")]
        public string darTicker
        {
            get
            {
                return _darTicker;
            }
            set
            {
                _darTicker = value.ToUpper();

            }
        }

        public string tier { get; set; }

        public decimal open { get; set; }

        public decimal high { get; set; }

        public decimal low { get; set; }

        public decimal close { get; set; }

        public decimal volume { get; set; }

        private string _windowStart;
        public string windowStart
        {
            get
            {
                return _windowStart;
            }
            set
            {
                var x = DateTime.ParseExact(value, "MM/dd/yyyy HH:mm:ss", null);
                _windowStart = x.ToString("yyyy-MM-ddTHH:mm:ss") + "+00:00";

            }
        }

        private string _windowEnd;
        public string windowEnd
        {
            get
            {
                return _windowEnd;
            }
            set
            {
                var x = DateTime.ParseExact(value, "MM/dd/yyyy HH:mm:ss", null);
                _windowEnd = x.ToString("yyyy-MM-ddTHH:mm:ss") + "+00:00";

            }
        }

        private string _effectiveTime;
        public string effectiveTime
        {
            get
            {
                return _effectiveTime;
            }
            set
            {
                var x = DateTime.ParseExact(value, "MM/dd/yyyy HH:mm:ss", null);
                _effectiveTime = x.ToString("yyyy-MM-ddTHH:mm:ss") + "+00:00";

            }
        }

        public override string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}
