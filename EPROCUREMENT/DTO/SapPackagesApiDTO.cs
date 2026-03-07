using System;
using System.Collections.Generic;

namespace EPROCUREMENT.DTO
{
    public class SapPackagesApiDTO
    {
        public string project_num { get; set; }

        public string datefrom { get; set; }

        public string dateTo { get; set; }

        public List<MaterialServiceDTO> inudstries { get; set; }
    }
}
