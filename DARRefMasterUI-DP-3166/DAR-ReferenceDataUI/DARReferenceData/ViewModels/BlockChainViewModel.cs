using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class BlockChainViewModel : DARViewModel
    {
        public string DARBlockchainID { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public string ConsensusMechanism { get; set; }

        public string HashAlgorithm { get; set; }
        public long Deleted { get; set; }
        public string Operation { get; set; }
        public override string GetDescription()
        {
            return $"{Name} - {Description} - {ConsensusMechanism}  - {HashAlgorithm}";
        }
    }
}