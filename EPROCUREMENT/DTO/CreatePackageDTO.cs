using EPROCUREMENT.Enums;
using System;
using System.Collections.Generic;

namespace EPROCUREMENT.DTO
{
	public class CreatePackageDTO
	{
		public string package_name { get; set; }

		public char assign_type { get; set; }

		public int project_id { get; set; }

		public int industry_id { get; set; }

		public int assigned_by { get; set; }

		public string filePath { get; set; }
        public string currency { get; set; }

		//public int credit_period { get; set; }
		//public PeriodType credit_periodType { get; set; }
		//public int delivery_duration { get; set; }
		//public PeriodType delivery_durationType { get; set; }
		//public DateTime expiration_date { get; set; }

        public List<SelectedRowsDTO> selected_rows { get; set; }
	}
}
