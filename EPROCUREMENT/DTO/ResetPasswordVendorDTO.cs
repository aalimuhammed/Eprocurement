using System.ComponentModel.DataAnnotations;

namespace EPROCUREMENT.DTO
{
    public class ResetPasswordVendorDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
