using AutoMapper;
using BudgetManagement.Models;
using BudgetManagement.Models.ViewModels;
using BudgetManagement.Services;
using BudgetManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IAccountRepository accountRepository;
        private readonly IMapper mapper;
        private readonly ITransactionRepository transactionRepository;
        private readonly IReportService reportService;
        private readonly IAccountTypeRepository accountTypeRepository;

        public AccountController(IAccountTypeRepository accountTypeRepository, IUserService userService,
            IAccountRepository accountRepository, IMapper mapper, ITransactionRepository transactionRepository,
            IReportService reportService)
        {
            this.accountTypeRepository = accountTypeRepository;
            this.userService = userService;
            this.accountRepository = accountRepository;
            this.mapper = mapper;
            this.transactionRepository = transactionRepository;
            this.reportService = reportService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = userService.getUserId();
            var accountsWithAccountType = await accountRepository.Find(userId);
            var model = accountsWithAccountType
                .GroupBy(x => x.AccountType)
                .Select(group => new AccountIndexViewModel
                {
                    AccountType = group.Key,
                    Accounts = group.AsEnumerable()
                }).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = userService.getUserId();
            var model = new AccountCreateViewModel();
            model.AccountTypes = await getAccountTypes(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountCreateViewModel account)
        {
            var userId = userService.getUserId();
            var accountTypes = await accountTypeRepository.GetById(account.AccountTypeId, userId);

            if (accountTypes is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (!ModelState.IsValid)
            {
                account.AccountTypes = await getAccountTypes(userId);
                return View(account);
            }

            await accountRepository.Create(account);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = userService.getUserId();
            var account = await accountRepository.GetById(id, userId);

            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var model = mapper.Map<AccountCreateViewModel>(account);

            model.AccountTypes = await getAccountTypes(userId);

            return View(model);
        }

        [HttpPost] 
        public async Task<IActionResult> Edit(AccountCreateViewModel accountToEdit)
        {
            var userId = userService.getUserId();
            var account = await accountRepository.GetById(accountToEdit.Id, userId);

            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var accountType = await accountTypeRepository.GetById(accountToEdit.AccountTypeId, userId);

            if(accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await accountRepository.Update(accountToEdit);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = userService.getUserId();
            var account = await accountRepository.GetById(id, userId);

            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userId = userService.getUserId();
            var account = await accountRepository.GetById(id, userId);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await accountRepository.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, int month, int year)
        {
            var userId = userService.getUserId();
            var account = await accountRepository.GetById(id, userId);

            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            ViewBag.AccountName = account.Name;

            var model = await reportService.getAccountDetailsReport(id, userId, month, year, ViewBag);

            return View(model);

        }

        private async Task<IEnumerable<SelectListItem>> getAccountTypes(int userId)
        {
            var accountTypes = await accountTypeRepository.Get(userId);
            return accountTypes.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }
    }
}
