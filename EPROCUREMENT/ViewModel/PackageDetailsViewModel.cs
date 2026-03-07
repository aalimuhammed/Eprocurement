using System;

namespace EPROCUREMENT.ViewModel
{
    public class PackageDetailsViewModel
    {
        public int id { get; set; }

        public string name { get; set; }

        public string comments { get; set; }

        public DateTime? start_date { get; set; }

        public DateTime? end_date { get; set; }

        public string file_name { get; set; }
    }
}
