using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.Models
{
    public class user_detail
    {
        [Key]
        public int id { get; set; }

        public int user_id { get; set; }

        public int type_id { get; set; }

        public int industries_details { get; set; }
    }
}
