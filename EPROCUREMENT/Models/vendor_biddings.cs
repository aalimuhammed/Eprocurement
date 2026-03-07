using System;
using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.Models
{
    public class vendor_biddings
    {
        [Key]
        public int id { get; set; }

        public int user_id { get; set; }

        public int pkg_details_id { get; set; }

        public DateTime? delivery_date { get; set; }

        public string advanced_payment { get; set; }

        public string technical_approval { get; set; }

        public string file_path { get; set; }
        public string  material_payment { get; set; }

        public string works_payment { get; set; }
        public string transportation { get; set; }
        public string duration_days { get; set; }

        public string price { get; set; }
    }
}
