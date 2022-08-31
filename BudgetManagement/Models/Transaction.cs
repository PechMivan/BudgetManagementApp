using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Transaction Date")]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public decimal Amount { get; set; }
        [StringLength(maximumLength: 1000)]
        public string Note { get; set; }
        [Display(Name = "Account")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "You must select an account!")]
        public int AccountId { get; set; }
        [Display(Name = "Category")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "You must select a category!")]
        public int CategoryId { get; set; }
        [Display(Name = "Operation Type")]
        public OperationType OperationTypeId { get; set; } = OperationType.Income;
        public string Account { get; set; }
        public string Category { get; set; }

    }
}
