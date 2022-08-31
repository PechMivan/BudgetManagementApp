using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "{0} field is required!")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} field is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
