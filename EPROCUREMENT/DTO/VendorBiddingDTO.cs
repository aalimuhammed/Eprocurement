using System;
using System.Collections.Generic;

namespace EPROCUREMENT.DTO
{
    public class VendorBiddingDTO
    {
		public List<BiddingSelectedRowsDTO> biddingSelectedRowsDTOs { get; set; }
		public DateTime? delivery_date { get; set; }
		public string advanced_payment { get; set; }
        public string filePath { get; set; }
        public string workspayment { get; set; }
        public string durationdays { get; set; }
        public string materialworks { get; set; }
        public string transportation { get; set; }
        public string comment { get; set; }
        public int user_id { get; set; }
	}
}