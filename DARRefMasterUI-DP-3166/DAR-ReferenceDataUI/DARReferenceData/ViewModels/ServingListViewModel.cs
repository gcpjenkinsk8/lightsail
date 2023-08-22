using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class ServingListViewModel: PairViewModel
    {
        public string ServingListID { get; set; }
        public string PairID { get; set; }
        public string SourceID { get; set; }
        public string ProcessID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public string StartDisplay { get; set; }
	    public string EndDisplay { get; set; }

        public string ProcessName { get; set; }
        public string ProcessDescription { get; set; }

        public int Lookback { get; set; }
        public string LookbackUnit { get; set; }
        public int Frequency { get; set; }

        public string FrequencyUnit { get; set; }


        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(PairID))
                return false;
            if (string.IsNullOrWhiteSpace(SourceID))
                return false;
            if (string.IsNullOrWhiteSpace(ProcessID))
                return false;

            return true;

        }

 
    }
}
