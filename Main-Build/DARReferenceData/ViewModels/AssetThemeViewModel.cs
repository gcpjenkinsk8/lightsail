using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class AssetThemeViewModel: DARViewModel
    {
        public string ID { get; set; }

        public string DARThemeID { get; set; }

        public string DARAssetID { get; set; }
        public string AssetName { get; set; }

        public string ThemeName { get; set; }

        public string ThemeType { get; set; }
        public string Operation { get; set; }
        public long Deleted { get; set; }
        public override string GetDescription()
        {
            return $"{DARAssetID} - {AssetName} - {ThemeName}";
        }
    }
}
