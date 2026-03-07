using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPROCUREMENT.Models
{
    public class packages_offers
    {
        public int id { get; set; }

        public int package_id { get; set; }

        public string offerfile_path { get; set; }

        public string offer_filename { get; set; }

        public int userid { get; set; }

        public string comments { get; set; }

        public bool verify { get; set; }

        public bool flag { get; set; }

        public bool pending { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }

    }
}
