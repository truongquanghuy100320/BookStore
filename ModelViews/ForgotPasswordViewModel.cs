using System.ComponentModel.DataAnnotations;

namespace BookStore.ModelViews
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        
    }

}
