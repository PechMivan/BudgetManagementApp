namespace BudgetManagement.Models.ViewModels
{
    public class PaginationResponse
    {
        public int PageIndex { get; set; } = 1;
        public int RecordsPerPage { get; set; } = 5;
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public string BaseURL { get; set; }
    }

    public class PaginationResponse<T> : PaginationResponse
    {
        public IEnumerable<T> Elements { get; set; }
    }
}
