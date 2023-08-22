using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class AppModuleViewModel: DARRoleViewModel
    {
        public string DARAppModuleID { get; set; }
        public string ModuleName { get; set; }

        public string ModuleDescription { get; set; }

        public string ModuleLink { get; set; }
    }
}
