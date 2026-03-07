using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPROCUREMENT.ViewModel
{
    public class AllApplied_ViewModel
    {
        public List<Dictionary<string, object>> Rows { get; set; }
        public List<string> Columns { get; set; }
    }
}
