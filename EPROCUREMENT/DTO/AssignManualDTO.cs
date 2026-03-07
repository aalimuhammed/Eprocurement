using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.DTO
{
	public class AssignManualDTO
	{
		public int Id { get; set; }

		public List<int> user_Id { get; set; }

		public string filepath { get; set; }
	}
}
