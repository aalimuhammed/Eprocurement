using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.ViewModel
{
    public class sap_project_ViewModel
    {
        [Key]
        public int ID { get; set; }

        public string proj_num { get; set; }

        public string proj_name { get; set; }
    }
}
