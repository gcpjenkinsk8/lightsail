using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
	public class ProcessViewModel : DARViewModel
	{
		public string DARProcessID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Lookback { get; set; }
		public string LookbackUnit { get; set; }
		public int Frequency { get; set; }

		public string FrequencyUnit { get; set; }




		public override string GetDescription()
        {
			return $"{Name} - {Description} - {Lookback}";
		}
    }
		
}
