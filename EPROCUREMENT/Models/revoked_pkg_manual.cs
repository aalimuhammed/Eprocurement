using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.Models
{
	public class revoked_pkg_manual
	{
		[Key]
		public int Id { get; set; }

		public int pkg_id { get; set; }
	}
}
