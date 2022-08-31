namespace BudgetManagement.Models.ViewModels
{
    public class MonthlyReportViewModel
    {
        public decimal Income => MonthlyTransactions.Sum(x => x.Income);
        public decimal Expense => MonthlyTransactions.Sum(x => x.Expense);
        public decimal Total => Income - Expense;
        public int Year { get; set; }
        public IEnumerable<MonthlyReportModel> MonthlyTransactions { get; set; }
    }
}
