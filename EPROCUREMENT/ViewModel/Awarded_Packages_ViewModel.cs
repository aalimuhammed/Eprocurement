using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.ViewModel
{
    public class Awarded_Packages_ViewModel
    {
        [Key]
        public int id { get; set; }

        public string pkg_name { get; set; }

        public string filename { get; set; }
    }
}
