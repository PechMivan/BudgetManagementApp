using BudgetManagement.Models;
using BudgetManagement.Models.ViewModels;

namespace BudgetManagement.Services
{
    public interface IReportService
    {
        Task<DetailReportViewModel> getAccountDetailsReport(int id, int userId, int month, int year, dynamic ViewBag);
        Task<DetailReportViewModel> GetUserDetailsReport(int userId, int month, int year, dynamic ViewBag);
        Task<IEnumerable<WeeklyReportModel>> GetWeeklyReport(int userId, int month, int year, dynamic ViewBag);
    }
    public class ReportService : IReportService
    {
        private readonly HttpContext httpContext;
        private readonly ITransactionRepository transactionRepository;

        public ReportService(IHttpContextAccessor httpContextAccesor, ITransactionRepository transactionRepository)
        {
            this.httpContext = httpContextAccesor.HttpContext;
            this.transactionRepository = transactionRepository;
        }

        public async Task<DetailReportViewModel> getAccountDetailsReport(int id, int userId, int month, int year, dynamic ViewBag)
        {
            (DateTime startDate, DateTime endDate) = getStartAndEndDates(month, year);

            var transactionsPerAccount = new TransactionPerAccount()
            {
                UserId = userId,
                AccountId = id,
                StartDate = startDate,
                EndDate = endDate
            };

            var transactions = await transactionRepository.GetByAccountId(transactionsPerAccount);

            var model = new DetailReportViewModel();
            model = GenerateDetailReportViewModel(startDate, endDate, transactions, model);

            AssignViewBagValues(ViewBag, startDate);

            return model;
        }

        public async Task<DetailReportViewModel> GetUserDetailsReport(int userId, int month, int year, dynamic ViewBag)
        {
            (DateTime startDate, DateTime endDate) = getStartAndEndDates(month, year);

            var transactionsPerUser = new TransactionPerUser()
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            };

            var transactions = await transactionRepository.GetByUserId(transactionsPerUser);

            var model = new DetailReportViewModel();
            model = GenerateDetailReportViewModel(startDate, endDate, transactions, model);

            AssignViewBagValues(ViewBag, startDate);

            return model;
        }

        public async Task<IEnumerable<WeeklyReportModel>> GetWeeklyReport(int userId, int month, int year, dynamic ViewBag)
        {
            (DateTime startDate, DateTime endDate) = getStartAndEndDates(month, year);

            var transactionsPerWeek = new TransactionPerUser()
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            };

            AssignViewBagValues(ViewBag, startDate);
            var model = await transactionRepository.GetReportsByWeek(transactionsPerWeek);

            return model;
        }

        private void AssignViewBagValues(dynamic ViewBag, DateTime startDate)
        {
            ViewBag.previousMonth = startDate.AddMonths(-1).Month;
            ViewBag.previousYear = startDate.AddMonths(-1).Year;
            ViewBag.nextMonth = startDate.AddMonths(1).Month;
            ViewBag.nextYear = startDate.AddMonths(1).Year;
            ViewBag.returnTo = httpContext.Request.Path + httpContext.Request.QueryString;
        }

        private (DateTime startDate, DateTime endDate) getStartAndEndDates(int month, int year)
        {
            DateTime startDate;
            DateTime endDate;

            if (month <= 0 || month > 12 || year <= 1900)
            {
                var today = DateTime.Today;
                startDate = new DateTime(today.Year, today.Month, 1);
            }
            else
            {
                startDate = new DateTime(year, month, 1);
            }

            endDate = startDate.AddMonths(1).AddDays(-1);

            return (startDate, endDate);
        }

        private DetailReportViewModel GenerateDetailReportViewModel(DateTime startDate, DateTime endDate, IEnumerable<Transaction> transactions, DetailReportViewModel model)
        {
            var transactionPerDate = transactions.OrderByDescending(x => x.TransactionDate)
                                                             .GroupBy(x => x.TransactionDate)
                                                             .Select(group => new DetailReportViewModel.TransactionPerDate()
                                                             {
                                                                 TransactionDate = group.Key,
                                                                 Transactions = group.AsEnumerable()
                                                             });

            model.GroupedTransactions = transactionPerDate;
            model.StartDate = startDate;
            model.EndDate = endDate;

            return model;
        }
    }
}
