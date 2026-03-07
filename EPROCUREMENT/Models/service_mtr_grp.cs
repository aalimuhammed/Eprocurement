using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.Models
{
    public class service_mtr_grp
    {
        [Key]
        public int id { get; set; }

        public int mtr_srv_id { get; set; }

        public string mtr_srv_grp_desc { get; set; }

        public string mtr_srv_grp_code { get; set; }
    }
}
