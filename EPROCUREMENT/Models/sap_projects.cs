using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.Models
{
    public class sap_projects
    {
        [Key]
        public int Id { get; set; }
        
        public string project_num { get; set; } 

        public string proj_name { get; set; }

        public string english_desc { get; set; }

        public int area_id { get; set; }
    }
}
