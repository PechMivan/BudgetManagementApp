namespace BudgetManagement.Models.ViewModels
{
    public class WeeklyReportViewModel
    {
        public decimal Income => WeeklyTransactions.Sum(x => x.Income);
        public decimal Expense => WeeklyTransactions.Sum(x => x.Expense);
        public decimal Total => Income - Expense;
        public DateTime ReferenceDate { get; set; }
        public IEnumerable<WeeklyReportModel> WeeklyTransactions { get; set; }
    }
}
