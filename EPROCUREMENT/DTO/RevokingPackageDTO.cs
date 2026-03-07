using System.Collections.Generic;

namespace EPROCUREMENT.DTO
{
	public class RevokingPackageDTO
	{
		public int project_id { get; set; }

		public int industry_id { get; set; }
		 
		public List<SelectedRowsDTO> selected_rows { get; set; }
	}
}
