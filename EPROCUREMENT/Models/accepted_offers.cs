using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.Models
{
    public class accepted_offers
    {
        [Key]
        public int id { get; set; }

        public int pkg_header_id { get; set; }

        public int user_id { get; set; }
    }
}
