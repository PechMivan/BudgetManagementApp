using AutoMapper;
using BudgetManagement.Models;
using BudgetManagement.Models.ViewModels;
using BudgetManagement.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace BudgetManagement.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IUserService userService;
        private readonly IAccountRepository accountRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;
        private readonly IReportService reportService;

        public TransactionController(ITransactionRepository transactionRepository, IUserService userService, 
            IAccountRepository accountRepository, ICategoryRepository categoryRepository, IMapper mapper,
            IReportService reportService)
        {
            this.transactionRepository = transactionRepository;
            this.userService = userService;
            this.accountRepository = accountRepository;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
            this.reportService = reportService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int month, int year)
        {
            var userId = userService.getUserId();
            var model = await reportService.GetUserDetailsReport(userId, month, year, ViewBag);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Weekly(int month, int year)
        {
            var userId = userService.getUserId();
            IEnumerable<WeeklyReportModel> weeklyTransactions = await reportService.GetWeeklyReport(userId, month, year, ViewBag);

            var GroupedWeeklyTransactions = weeklyTransactions.GroupBy(x => x.Week)
                                            .Select(group => new WeeklyReportModel()
                                            {
                                                Week = group.Key,
                                                Income = group.Where(x => x.OperationTypeId == OperationType.Income).Select(x => x.Amount).FirstOrDefault(),
                                                Expense = group.Where(x => x.OperationTypeId == OperationType.Expense).Select(x => x.Amount).FirstOrDefault()
                                            }).ToList();

            if(year == 0 || month == 0)
            {
                var today = DateTime.Today;
                month = today.Month;
                year = today.Year;
            }

            var referenceDate = new DateTime(year, month, 1);
            var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(year, month));

            var weeks = daysInMonth.Chunk(7).ToList();

            for (int i = 0; i < weeks.Count(); i++)
            {
                var week = i + 1;
                var startDate = new DateTime(year, month, weeks[i].First());
                var endDate = new DateTime(year, month, weeks[i].Last());
                var groupOfWeek = GroupedWeeklyTransactions.FirstOrDefault(x => x.Week == week);

                if (groupOfWeek is null)
                {
                    GroupedWeeklyTransactions.Add(new WeeklyReportModel()
                    {
                        Week = week,
                        StartDate = startDate,
                        EndDate = endDate
                    });
                }
                else
                {
                    groupOfWeek.StartDate = startDate;
                    groupOfWeek.EndDate = endDate;
                }
            }

                GroupedWeeklyTransactions.OrderByDescending(x => x.Week).ToList();

                var model = new WeeklyReportViewModel()
                {
                    WeeklyTransactions = GroupedWeeklyTransactions,
                    ReferenceDate = referenceDate
                };

                return View(model);
            }

        [HttpGet]
        public async Task<IActionResult> Monthly(int year)
        {
            var userId = userService.getUserId();
            
            if(year == 0)
            {
                year = DateTime.Today.Year;
            }

            var monthlyTransactions = await transactionRepository.GetReportsByMonth(userId, year);

            var GroupedMonthlyTransactions = monthlyTransactions.GroupBy(x => x.Month)
                                            .Select(group => new MonthlyReportModel()
                                            {
                                                Month = group.Key,
                                                Income = group.Where(x => x.OperationTypeId == OperationType.Income).Select(x => x.Amount).FirstOrDefault(),
                                                Expense = group.Where(x => x.OperationTypeId == OperationType.Expense).Select(x => x.Amount).FirstOrDefault()
                                            }).ToList();

            
            for (int month = 1; month <= 12; month++)
            {
                var groupOfMonth = GroupedMonthlyTransactions.FirstOrDefault(x => x.Month == month);
                var referenceDate = new DateTime(year, month, 1);

                if(groupOfMonth is null)
                {
                    GroupedMonthlyTransactions.Add(new MonthlyReportModel()
                    {
                        Month = month,
                        ReferenceDate = referenceDate
                    });
                }
                else
                {
                    groupOfMonth.ReferenceDate = referenceDate;
                }
            }

            GroupedMonthlyTransactions = GroupedMonthlyTransactions.OrderBy(x => x.Month).ToList();

            var model = new MonthlyReportViewModel()
            {
                MonthlyTransactions = GroupedMonthlyTransactions,
                Year = year
            };

            return View(model);
        }

        public async Task<IActionResult> ExportMonthlyReportToExcel(int month, int year)
        {
            var userId = userService.getUserId();
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var transactions = await transactionRepository.GetByUserId(new TransactionPerUser()
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            });

            var filename = $"Budget Management[{startDate.ToString("MMM - yyyy")}].xlsx";

            return generateExcel(filename, transactions);
        }

        private FileResult generateExcel(string filename, IEnumerable<Transaction> transactions)
        {
            DataTable datatable = new DataTable("Transactions");
            datatable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Date"),
                new DataColumn("Account"),
                new DataColumn("Category"),
                new DataColumn("Operation Type"),
                new DataColumn("Amount"),
                new DataColumn("Note")
            });

            foreach (var transaction in transactions)
            {
                datatable.Rows.Add(
                    transaction.TransactionDate, 
                    transaction.Account, 
                    transaction.Category, 
                    transaction.OperationTypeId, 
                    transaction.Amount, 
                    transaction.Note);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(datatable);

                using(MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(
                        stream.ToArray(), 
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                        filename);
                }
            }
        }

        [HttpGet]
        public IActionResult Calendar()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetCalendarEvents(DateTime start, DateTime end)
        {
            var userId = userService.getUserId();

            var transactions = await transactionRepository.GetByUserId(new TransactionPerUser
            {
                UserId = userId,
                StartDate = start,
                EndDate = end
            });

            var calendarEvents = transactions.Select(transaction => new CalendarEventModel()
            {
                Title = transaction.Amount.ToString("N"),
                Start = transaction.TransactionDate.ToString("yyyy-MM-dd"),
                End = transaction.TransactionDate.ToString("yyyy-MM-dd"),
                Color = (transaction.OperationTypeId == OperationType.Expense ? "Red" : null)
            });

            return Json(calendarEvents);
        }

        [HttpGet]
        public async Task<JsonResult> FetchCalendarTransactions(DateTime date)
        {
            var userId = userService.getUserId();

            var transactions = await transactionRepository.GetByUserId(new TransactionPerUser
            {
                UserId = userId,
                StartDate = date,
                EndDate = date
            });

            return Json(transactions);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = userService.getUserId();
            var model = new TransactionCreateViewModel();
            model.Accounts = await getAccounts(userId);
            model.Categories = await getCategories(userId, model.OperationTypeId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionCreateViewModel transaction)
        {
            var userId = userService.getUserId();

            if (!ModelState.IsValid)
            {
                var model = new TransactionCreateViewModel();
                model.Accounts = await getAccounts(userId);
                model.Categories = await getCategories(userId, model.OperationTypeId);
                return View(model);
            }

            var account = await accountRepository.GetById(transaction.AccountId, userId);

            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var category = await categoryRepository.GetById(transaction.CategoryId, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if(transaction.OperationTypeId == OperationType.Expense)
            {
                transaction.Amount *= -1;
            }

            transaction.UserId = userId;
            await transactionRepository.Create(transaction);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string returnTo = null)
        {
            var userId = userService.getUserId();
            var transaction = await transactionRepository.GetById(id, userId);

            if(transaction is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var model = mapper.Map<TransactionUpdateViewModel>(transaction);
            model.previousAmount = transaction.Amount;

            if(transaction.OperationTypeId == OperationType.Expense)
            {
                model.previousAmount *= -1;
            }

            model.previousAccountId = transaction.AccountId;
            model.Accounts = await getAccounts(userId);
            model.Categories = await getCategories(userId, transaction.OperationTypeId);
            model.returnTo = returnTo;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TransactionUpdateViewModel model)
        {
            var userId = userService.getUserId();

            if (!ModelState.IsValid)
            {
                model.Accounts = await getAccounts(userId);
                model.Categories = await getCategories(userId, model.OperationTypeId);
                return View(model);
            }

            var account = await accountRepository.GetById(model.AccountId, userId);

            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var category = await accountRepository.GetById(model.AccountId, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var transaction = mapper.Map<Transaction>(model);

            if(transaction.OperationTypeId == OperationType.Expense)
            {
                transaction.Amount *= -1;
            }

            await transactionRepository.Update(transaction, model.previousAccountId, model.previousAmount);

            if(string.IsNullOrEmpty(model.returnTo))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(model.returnTo);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string returnTo = null)
        {
            var userId = userService.getUserId();
            var transaction = await transactionRepository.GetById(id, userId);

            if(transaction is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await transactionRepository.Delete(id);

            if (string.IsNullOrEmpty(returnTo))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(returnTo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> getCategories([FromBody] OperationType operationTypeId)
        {
            var userId = userService.getUserId();
            var categories = await getCategories(userId, operationTypeId);
            return Ok(categories);
        } 

        private async Task<IEnumerable<SelectListItem>> getAccounts(int userId)
        {
            var accounts = await accountRepository.Find(userId);
            return accounts.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> getCategories(int userId, OperationType operationTypeId)
        {
            var categories = await categoryRepository.Get(userId, operationTypeId);
            return categories.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }
    }
}
