using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.Models
{
    public class package_manual
    {
        [Key]
        public int id { get; set; }

        public string serial { get; set; }

        public string material_description { get; set; }

        public string uom { get; set; }

        public string qty { get; set; }

        public int project_id { get; set; }

        public int industry_id { get; set; }

        public int pack_no { get; set; }

        public string package_name { get; set; }

	}
}
