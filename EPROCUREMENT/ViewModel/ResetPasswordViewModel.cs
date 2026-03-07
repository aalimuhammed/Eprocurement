using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.ViewModel
{
    public class ResetPasswordViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
