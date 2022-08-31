namespace BudgetManagement.Models.ViewModels
{
    public class DetailReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<TransactionPerDate> GroupedTransactions { get; set; }
        public decimal TotalIncomeBalance => GroupedTransactions.Sum(x => x.IncomeBalance);
        public decimal TotalExpenseBalance => GroupedTransactions.Sum(x => x.ExpenseBalance);
        public decimal TotalBalance => TotalIncomeBalance - TotalExpenseBalance;

        public class TransactionPerDate
        {
            public DateTime TransactionDate { get; set; }
            public IEnumerable<Transaction> Transactions { get; set; }
            public decimal IncomeBalance => Transactions.Where(x => x.OperationTypeId == OperationType.Income)
                                                        .Sum(x => x.Amount);
            public decimal ExpenseBalance => Transactions.Where(x => x.OperationTypeId == OperationType.Expense)
                                                        .Sum(x => x.Amount);
        }
    }
}
