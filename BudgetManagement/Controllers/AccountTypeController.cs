using BudgetManagement.Models;
using BudgetManagement.Models.ViewModels;
using BudgetManagement.Services;
using Microsoft.AspNetCore.Mvc;
namespace BudgetManagement.Controllers
{
    public class AccountTypeController : Controller
    {
        private readonly IAccountTypeRepository accountTypeRepository;
        private readonly IUserService userService;

        public AccountTypeController(IAccountTypeRepository accountTypeRepository, IUserService userService)
        {
            this.accountTypeRepository = accountTypeRepository;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(PaginationViewModel pagination)
        {
            var userId = userService.getUserId();
            var totalAccountTypes = await accountTypeRepository.Count(userId);

            if (pagination.RecordsPerPages < 1)
            {
                pagination.RecordsPerPages = 5;
            }

            var totalPages = (int)Math.Ceiling((double)totalAccountTypes / pagination.RecordsPerPages);

            if (pagination.PageIndex < 1)
            {
                pagination.PageIndex = 1;
            }
            else if (pagination.PageIndex > totalPages)
            {
                pagination.PageIndex = totalPages;
            }

            var accountTypes = await accountTypeRepository.GetPagination(userId, pagination);

            var model = new PaginationResponse<AccountType>()
            {
                PageIndex = pagination.PageIndex,
                TotalPages = totalPages,
                RecordsPerPage = pagination.RecordsPerPages,
                Elements = accountTypes,
                TotalRecords = totalAccountTypes,
                BaseURL = Url.Action()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountType accountType)
        {
            if (!ModelState.IsValid)
            {
                return View(accountType);
            }

            accountType.UserId = userService.getUserId();

            var AccountTypeAlreadyExists = await accountTypeRepository.Exists(accountType.Name, accountType.UserId);

            if(AccountTypeAlreadyExists)
            {
                ModelState.AddModelError(nameof(accountType.Name), 
                                         $"Name {accountType.Name} is already used");
                return View(accountType);
            }

            await accountTypeRepository.Create(accountType);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = userService.getUserId();
            var accountType = await accountTypeRepository.GetById(id, userId);

            if(accountType is null)
            {
                return RedirectToAction("ItemNotFound", "Home");
            }

            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountType accountType)
        {
            var userId = userService.getUserId();
            var AccountTypeExists = await accountTypeRepository.GetById(accountType.Id, userId);

            if(AccountTypeExists is null)
            {
                return RedirectToAction("ItemNotFound", "Home");
            }

            await accountTypeRepository.Update(accountType);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = userService.getUserId();
            var accountType = await accountTypeRepository.GetById(id, userId);

            if(accountType is null)
            {
                return RedirectToAction("ItemNotFound", "Home");
            }

            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccountType(int id)
        {
            var userId = userService.getUserId();
            var accountType = await accountTypeRepository.GetById(id, userId);

            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await accountTypeRepository.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AccountTypeAlreadyExists(string name)
        {
            var UserId = userService.getUserId();
            var AccountTypeAlreadyExists = await accountTypeRepository.Exists(name, UserId);

            if (AccountTypeAlreadyExists)
            {
                return Json($"Name {name} is already used");
            }

            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> Sort([FromBody] int[] ids)
        {
            var userId = userService.getUserId();
            var accountTypes = await accountTypeRepository.Get(userId);
            var accountTypesIds = accountTypes.Select(x => x.Id);

            var nonUserIds = ids.Except(accountTypesIds).ToList();

            if (nonUserIds.Any())
            {
                Forbid();
            }

            var accountTypesSorted = ids.Select((value, index) =>
                new AccountType() { Id = value, Order = index + 1 }).AsEnumerable();

            await accountTypeRepository.Sort(accountTypesSorted);

            return Ok();
        }
    }
}
