using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.ViewModel
{
	public class ImportedPkgs_ViewModel
	{
		[Key]
		public int Id { get; set; }
		public string pkg_name { get; set; }
	}
}