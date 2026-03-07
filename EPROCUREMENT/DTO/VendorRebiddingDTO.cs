using System.Collections.Generic;
using System;

namespace EPROCUREMENT.DTO
{
    public class VendorRebiddingDTO
    {
        public List<ReBiddingSelectedRowsDTO> biddingSelectedRowsDTOs { get; set; }
        public DateTime? delivery_date { get; set; }
        public string workspayment { get; set; }
        public string durationdays { get; set; }
        public string materialworks { get; set; }
        public string transportation { get; set; }
        public string advanced_payment { get; set; }
        public string filePath { get; set; }
        public int id { get; set; }
    }
}