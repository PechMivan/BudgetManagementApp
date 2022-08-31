using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "{0} field is required!")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} field is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
