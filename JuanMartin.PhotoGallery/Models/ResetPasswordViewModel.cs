using System.ComponentModel.DataAnnotations;
namespace JuanMartin.PhotoGallery.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "New password required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password does not match")]
        public string ConfirmPassword { get; set; }

        public string ResetCode { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
