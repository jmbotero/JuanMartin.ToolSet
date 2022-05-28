using System.ComponentModel.DataAnnotations;

namespace JuanMartin.PhotoGallery.Models
{
    public class LoginViewModel
    {
        public int UserId { get; private set; }
        [Required(ErrorMessage = "Please Provide Username", AllowEmptyStrings = false)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please provide password", AllowEmptyStrings = false)]
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
