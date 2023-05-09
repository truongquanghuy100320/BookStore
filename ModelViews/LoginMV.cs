using System.ComponentModel.DataAnnotations;

namespace BookStore.ModelViews
{
    public class LoginMV
    {
        
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        

    }
}
