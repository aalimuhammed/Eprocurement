using AspNetCoreAutoMapperDemo.Mappings;
using AutoMapper;
using EPROCUREMENT.Models;
using Microsoft.AspNetCore.Http;
using System;

namespace EPROCUREMENT.DTO
{
    public class ProjectPackageDTO : IMapFrom<proj_packages>
    {
        public int id { get; set; }

        public int prj_id { get; set; }

        public int boq_cpt_id { get; set; }

   
        public IFormFile File { get; set; }
        public DateTime start_date { get; set; }

        public DateTime end_date { get; set; }

        public string comments { get; set; }

        public string link_of_drive { get; set; }


        public void Mapping(Profile profile)
        {
            var c = profile.CreateMap<ProjectPackageDTO, proj_packages>()
                .ForMember(d => d.id, opt => opt.Ignore());
    

        }
    }
}
