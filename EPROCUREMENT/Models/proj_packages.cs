using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPROCUREMENT.Models
{
    public class proj_packages
    {
        public int id { get; set; }

        public int prj_id { get; set; }

        [NotMapped]
        public string project_name { get; set; }

        public int boq_cpt_id { get; set; }

        public string file_path { get; set; }

        public string file_name { get; set; }

        public DateTime? start_date { get; set; }

        public DateTime? end_date { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }
        public string comments { get; set; }

        public string link_of_drive { get; set; }

	}
}
