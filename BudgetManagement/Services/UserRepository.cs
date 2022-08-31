using BudgetManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BudgetManagement.Services
{
    public interface IUserRepository
    {
        Task<int> CreateUser(User user);
        Task<User> GetUserByEmail(string normalizedEmail);
    }
    public class UserRepository : IUserRepository
    {
        private readonly string connectionString;
        public UserRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateUser(User user)
        {
            var connection = new SqlConnection(connectionString);
            var userId = await connection.QuerySingleAsync<int>(@"INSERT INTO Users(Email, NormalizedEmail, PasswordHash)
                                                            VALUES(@Email, @NormalizedEmail, @PasswordHash) SELECT SCOPE_IDENTITY()", user);

            await connection.ExecuteAsync("Populate_Database", new { userId }, commandType: System.Data.CommandType.StoredProcedure);

            return userId;
        }

        public async Task<User> GetUserByEmail(string normalizedEmail)
        {
            var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Email = @NormalizedEmail", new {normalizedEmail });
        }
    }
}
