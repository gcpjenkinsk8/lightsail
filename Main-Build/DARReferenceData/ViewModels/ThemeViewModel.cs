using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class ThemeViewModel: DARViewModel
    {
        public string DARThemeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string ThemeType { get; set; }
        public string Operation { get; set; }
        public long Deleted { get; set; }

        public override string GetDescription()
        {
            return $"{Name} - {Description} - {ThemeType} - {DARThemeID}";
        }
    }
}
