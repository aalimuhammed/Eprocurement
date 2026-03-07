using System;

namespace EPROCUREMENT.ViewModel
{
    public class VendorPackageDetailsViewModel
    {
        public int id { get; set; }

        public string project_name { get; set; }

        public DateTime? startdate { get; set; }

        public DateTime? enddate { get; set; }

        public string filename { get; set; }

        public string comment { get; set; }


    }
}
