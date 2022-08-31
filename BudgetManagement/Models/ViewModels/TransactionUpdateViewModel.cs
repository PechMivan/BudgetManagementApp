namespace BudgetManagement.Models.ViewModels
{
    public class TransactionUpdateViewModel : TransactionCreateViewModel
    {
        public int previousAccountId { get; set; }
        public decimal previousAmount { get; set; }
        public string returnTo { get; set; }
    }
}
