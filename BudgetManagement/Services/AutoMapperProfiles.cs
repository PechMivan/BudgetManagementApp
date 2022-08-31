using AutoMapper;
using BudgetManagement.Models;
using BudgetManagement.Models.ViewModels;
using BudgetManagement.ViewModels;

namespace BudgetManagement.Services
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Account, AccountCreateViewModel>();
            CreateMap<TransactionUpdateViewModel, Transaction>().ReverseMap();
        }
    }
}
