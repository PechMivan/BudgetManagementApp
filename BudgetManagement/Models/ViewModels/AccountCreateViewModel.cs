using BudgetManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetManagement.ViewModels
{
    public class AccountCreateViewModel : Account
    {
        public IEnumerable<SelectListItem> AccountTypes { get; set; }
    }
}
