using BudgetManagement.Models;
using BudgetManagement.Models.ViewModels;
using BudgetManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManagement.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUserService userService;
        private readonly ICategoryRepository categoryRepository;

        public CategoryController(IUserService userService, ICategoryRepository categoryRepository)
        {
            this.userService = userService;
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(PaginationViewModel pagination)
        {
            var userId = userService.getUserId();
            var totalCategories = await categoryRepository.Count(userId);


            if (pagination.RecordsPerPages < 1)
            {
                pagination.RecordsPerPages = 5;
            }

            var totalPages = (int)Math.Ceiling((double)totalCategories / pagination.RecordsPerPages);

            if (pagination.PageIndex < 1)
            {
                pagination.PageIndex = 1;
            }
            else if (pagination.PageIndex > totalPages)
            {
                pagination.PageIndex = totalPages;
            }

            var categories = await categoryRepository.Get(userId, pagination);

            var model = new PaginationResponse<Category>()
            {
                PageIndex = pagination.PageIndex,
                TotalPages = totalPages,
                RecordsPerPage = pagination.RecordsPerPages,
                Elements = categories,
                TotalRecords = totalCategories,
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
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var userId = userService.getUserId();
            category.UserId = userId;
            await categoryRepository.Create(category);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = userService.getUserId();
            var category = await categoryRepository.GetById(id, userId);

            if(category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category categoryToEdit)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryToEdit);
            }

            var userId = userService.getUserId();
            var category = await categoryRepository.GetById(categoryToEdit.Id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            categoryToEdit.UserId = userId;
            await categoryRepository.Update(categoryToEdit);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = userService.getUserId();
            var category = await categoryRepository.GetById(id, userId);

            if(category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = userService.getUserId();
            var category = await categoryRepository.GetById(id, userId);

            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await categoryRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
