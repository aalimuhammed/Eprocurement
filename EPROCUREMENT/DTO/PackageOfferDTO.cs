using AutoMapper;
using EPROCUREMENT.Models;
using Microsoft.AspNetCore.Http;
using System;
using AspNetCoreAutoMapperDemo.Mappings;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPROCUREMENT.DTO
{
    public class PackageOfferDTO : IMapFrom<packages_offers>
    {

        public int id { get; set; }

        public string project_name { get; set; }
        public int package_id { get; set; }

        public string file_name { get; set; }

        public string file_path { get; set; }

        public int user_id { get; set; }

        [NotMapped]
        public string comments_s { get; set; }

        public string comments { get; set; }

        public bool verify { get; set; }

        public bool flag { get; set; }

        public IFormFile File { get; set; }

        public bool pending { get; set; }

        [NotMapped]

        public DateTime? start_date { get; set; }
        [NotMapped]
        public DateTime? end_date { get; set; }

        public void Mapping(Profile profile)
        {
            var c = profile.CreateMap<PackageOfferDTO, users_offers>()
                .ForMember(d => d.id, opt => opt.Ignore());
        }
    }
}
