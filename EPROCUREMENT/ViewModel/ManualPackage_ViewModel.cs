using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.ViewModel
{
	public class ManualPackage_ViewModel
	{
        public int id { get; set; }

		//public int PkgID { get; set; }

		//public string pkg_name { get; set; }

		public string mtr_uom { get; set; }

		public int serial { get; set; }

		public string mtr_qty {  get; set; }

		public string mtr_desc { get; set; }

		//public int pack_no { get; set; }

	}
}