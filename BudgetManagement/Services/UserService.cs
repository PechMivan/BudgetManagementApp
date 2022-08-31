using System.Security.Claims;

namespace BudgetManagement.Services
{
    public interface IUserService
    {
        int getUserId();
    }
    public class UserService : IUserService
    {
        private readonly HttpContext httpContext;
        public UserService(IHttpContextAccessor httpContextAccesor)
        {
            httpContext = httpContextAccesor.HttpContext;
        }

        public int getUserId()
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier)
                                                     .FirstOrDefault();
                var id = int.Parse(idClaim.Value);

                return id;
            }
            else
            {
                throw new ApplicationException("User is not authenticated");
            }
        }
    }
}
