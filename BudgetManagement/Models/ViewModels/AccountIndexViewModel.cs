namespace BudgetManagement.Models.ViewModels
{
    public class AccountIndexViewModel
    {
        public string AccountType { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public decimal TotalBalance => Accounts.Sum(x => x.Balance);
    }
}
