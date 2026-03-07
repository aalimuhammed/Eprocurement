using System.Collections.Generic;

namespace EPROCUREMENT.DTO
{
	public class RevokingPackageManualDTO
	{
		public int project_id { get; set; }

		public int industry_id { get; set; }

		public List<SelectedRowsManualDTO> selected_rows { get; set; }

	}

	public class Keys
	{
		public int id { get; set; }
	}
}
