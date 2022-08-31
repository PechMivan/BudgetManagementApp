namespace BudgetManagement.Models
{
    public class TransactionPerUser
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
