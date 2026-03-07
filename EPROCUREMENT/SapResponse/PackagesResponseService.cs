using System;

namespace EPROCUREMENT.SapResponse
{
	public class PackagesResponseService
	{
		public string PR { get; set; }
		public int PR_ITEM { get; set; }
		public int SERITEM { get; set; }
		public DateTime PR_REQ_DATE { get; set; }
		public string SER_CODE { get; set; }
		public string S_GRP { get; set; }
		public string SER_DESC { get; set; }
		public string S_LON_DES { get; set; }
		public double S_REQ_QTY { get; set; }
		public string S_UNIT { get; set; }
        public string TRACK_PRO { get; set; }
    }
}
