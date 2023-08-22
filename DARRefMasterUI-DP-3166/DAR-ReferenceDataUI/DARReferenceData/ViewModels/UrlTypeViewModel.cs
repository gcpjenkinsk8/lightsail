using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class UrlTypeViewModel: DARViewModel
    {

		public string DARURLTypeID { get; set; }

        public string Name { get; set; }
		public string DisplayName { get; set; }
		public string APIName { get; set; }
		public string Operation { get; set; }
		public long Deleted { get; set; }

		public override string GetDescription()
		{
			return $"{Name} - {DisplayName} - {APIName}";
		}
	}
}
