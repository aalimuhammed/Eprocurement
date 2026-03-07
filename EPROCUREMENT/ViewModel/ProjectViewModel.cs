using System;

namespace EPROCUREMENT.ViewModel
{
    public class ProjectViewModel
    {
        public int id { get; set; }

        public string project_numb { get; set; }

        public string project_name { get; set; }

        public DateTime start_date { get; set; }

        public DateTime end_date { get; set; }

        public string status { get; set; }

        public string area_name { get; set; }

        public int CountPackage { get; set; }

    }
}
