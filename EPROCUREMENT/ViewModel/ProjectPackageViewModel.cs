using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.ViewModel
{
    [Keyless]
    public class ProjectPackageViewModel
    {

        public int chaptr_id { get; set; }
        public string BOQ_Standard { get; set; }
        public int package_no { get; set; }

    }
}
