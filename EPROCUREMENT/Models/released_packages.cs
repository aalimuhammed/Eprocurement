using System.ComponentModel.DataAnnotations;
using System;

namespace EPROCUREMENT.Models
{
	public class released_packages
	{
		[Key]
		public int id { get; set; }
		public string pr_num { get; set; }
		public string line_item { get; set; }
		public DateTime? pr_date { get; set; }
		public string mtr_code { get; set; }
		public string m_grp { get; set; }
		public string mtr_desc { get; set; }
		public string mtr_long_desc { get; set; }
		public string mtr_batch { get; set; }
		public string mtr_qty { get; set; }
		public string mtr_uom { get; set; }
		public int project_id { get; set; }
		public int industry_id { get; set; }
	}
}
