using System.Collections.Generic;

namespace EPROCUREMENT.DTO
{
	public class CreatePackageManualDTO
	{
		public string package_name { get; set; }

		public char assign_type { get; set; }

		public int project_id { get; set; }

		public int industry_id { get; set; }

		public int assigned_by { get; set; }

		public string filePath { get; set; }
        public string currency { get; set; }

        public List<SelectedRowsDTO> selected_rows { get; set; }
		public List<int> user_id { get; set; }
	}
}