using System;

namespace EPROCUREMENT.ViewModel
{
	public class price_comparison_vm
	{
		public string mtr_desc { get; set; }

		public string mtr_uom { get; set; }

		public string mtr_qty { get; set; }

		public string price { get; set; }

		public string fname { get; set; }

		public string Offer { get; set; }

		public int userid { get; set; }

		public string duration_days { get; set; }

		public DateTime delivery_date { get; set; }

		public string advanced_payment { get; set; }

		public string transportation { get; set; }

		public string line_item { get; set; }
    }
}
