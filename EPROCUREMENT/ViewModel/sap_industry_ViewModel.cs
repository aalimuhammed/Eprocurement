using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.ViewModel
{
    public class sap_industry_ViewModel
    {
        [Key]
        public int id { get; set; }

        public string industry_code { get; set;}

        public string descr { get; set; }
    }
}
