using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "{0} field is required!")]
        [StringLength(maximumLength: 50)]
        public string Name { get; set; }
        public int UserId { get; set; }
        [Display(Name="Operation Type")]
        public OperationType OperationTypeId { get; set; }
    }
}
