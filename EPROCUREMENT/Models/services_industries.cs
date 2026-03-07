using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.Models
{
    public class services_industries
    {
        [Key]
        public int id { get; set; }
        public string industry_code { get; set; }
        public string descr { get; set; }
        public string english_desc { get; set; }
        public int header_id { get; set; }
    }
}
