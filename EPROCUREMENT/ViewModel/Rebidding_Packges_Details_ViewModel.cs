using System;

namespace EPROCUREMENT.ViewModel
{
    public class Rebidding_Packges_Details_ViewModel
    {
        public int Id { get; set; } 
        public string price { get; set; }
        public DateTime? delivery_date { get; set; }
        public string advanced_payment { get; set; }

        public string filename { get; set; }

        public string mtr_desc { get; set; }

        public string mtr_qty { get; set; }

        public string mtr_uom { get; set; }

        public string material_payment { get; set; }

        public string works_payment { get; set; }

        public string duration_days { get; set; }

        public string transportation { get ; set; }

        public string mtr_long_desc { get; set; }

    }
}
