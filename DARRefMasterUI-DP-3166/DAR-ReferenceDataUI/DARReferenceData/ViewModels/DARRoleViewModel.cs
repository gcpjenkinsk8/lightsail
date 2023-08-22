using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class DARRoleViewModel: DARViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Operation { get; set; }
        public long Deleted { get; set; }
        public override string GetDescription()
        {
            return RoleName;
        }
    }
}
