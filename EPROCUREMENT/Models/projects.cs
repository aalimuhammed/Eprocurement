using System;

namespace EPROCUREMENT.Models
{
    public class projects
    {
        public int id { get; set; }

        public string project_numb { get; set; }

        public string project_name { get; set; }

        public string delivery_point { get; set; }

        public DateTime start_date { get; set; }

        public DateTime end_date { get; set; }

        public bool status { get; set; }

        public int area_id { get; set; }
    }
}
