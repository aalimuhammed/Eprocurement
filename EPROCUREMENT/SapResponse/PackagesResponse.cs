using System;

namespace EPROCUREMENT.SapResponse
{
	public class PackagesResponse
	{
		public string PR { get; set; }
		public int PR_ITEM { get; set; }
		public DateTime PR_REQ_DATE { get; set; }
		public string MAT_CODE { get; set; }
		public string M_GRP { get; set; }
		public string MAT_DESC { get; set; }
		public string M_LON_DES { get; set; }
		public string M_BATCH { get; set; }
		public double M_REQ_QTY { get; set; }
		public string M_UNIT { get; set; }
	}
}
