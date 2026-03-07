using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.Models
{
    public class service_header
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }

        public string english_desc { get; set; }
    }
}
