using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.Models
{
	public class packages_header
	{
		[Key]
		public int id { get; set; }

		public string pkg_name { get; set; }

		public string file_path { get; set; }
		 
		public char assign_type { get; set; }

		public int project_id { get; set; }

		public int industry_id { get; set; }

		public int pkg_num { get; set; }

		public bool bid { get; set; }
		public bool cancelled { get; set; }
		public bool excl_import { get; set; }
		public int? assigned_by { get; set; }
		public bool is_service { get; set; }
        public string currency { get; set; }
    }
}