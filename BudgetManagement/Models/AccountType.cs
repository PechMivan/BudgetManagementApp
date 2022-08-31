using BudgetManagement.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models
{
    public class AccountType
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "{0} field is required!")]
        [StringLength(maximumLength: 50)]
        [FirstLetterCapitalized]
        [Remote(action: "AccountTypeAlreadyExists", controller: "AccountType")]
        public String Name { get; set; }
        public int UserId { get; set; }
        public int Order { get; set; }
    }
}
