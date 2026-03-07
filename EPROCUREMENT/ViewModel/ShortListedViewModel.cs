using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPROCUREMENT.ViewModel
{
    public class ShortListedViewModel
    {
        [NotMapped]
        public List<string> Vendors { get; set; }

        [NotMapped]
        public Dictionary<string, string> MtrDescPrices { get; set; }
    }
}
