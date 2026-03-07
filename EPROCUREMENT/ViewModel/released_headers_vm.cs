using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.ViewModel
{
    public class released_headers_vm
    {
        [Key]
        public int id { get; set; }

        public string pkg_name { get; set; }

        public char assign_type { get; set; }

        public string filename { get; set; }

        public string HasUser { get; set; }

        public bool bid { get; set; }

        public bool cancelled { get; set; }

		public string admin { get; set; }
    }
}
