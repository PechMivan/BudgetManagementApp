namespace BudgetManagement.Models.ViewModels
{
    public class PaginationViewModel
    {
        public int PageIndex { get; set; } = 1;
        private int RecordsPerPage { get; set; } = 5;
        private readonly int MaxRecordsPerPage = 50;
        

        public int RecordsPerPages
        {
            get
            {
                return RecordsPerPage;
            }
            set
            {
                RecordsPerPage = (value > MaxRecordsPerPage) ? MaxRecordsPerPage : value;
            }
        }

        public int Offset => RecordsPerPage * (PageIndex - 1);
    }
}
