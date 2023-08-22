using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class LogViewModel: DARViewModel
    {
        public string ApplicationName { get; set; }
        public string MessageType { get; set; }
        public string Message { get; set; }

        public string LoadTime { get; set; }
        public override string GetDescription()
        {
            return $"{MessageType}: {Message}";
        }
    }
}
