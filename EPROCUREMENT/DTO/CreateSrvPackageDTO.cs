using System.Collections.Generic;

namespace EPROCUREMENT.DTO
{
    public class CreateSrvPackageDTO
    {
        public string package_name { get; set; }
        public char assign_type { get; set; }
        public int project_id { get; set; }
        public int industry_id { get; set; }
        public int assigned_by { get; set; }
        public string filePath { get; set; }
        public string currency { get; set; }
        public List<SelectedRowsSrvDTO> selected_rows { get; set; }
    }
}
