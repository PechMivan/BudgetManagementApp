using BudgetManagement.Models;
using BudgetManagement.Models.ViewModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BudgetManagement.Services
{
    public interface IAccountTypeRepository
    {
        Task<int> Count(int userId);
        Task Create(AccountType accountType);
        Task Delete(int id);
        Task<bool> Exists(string name, int userId);
        Task<IEnumerable<AccountType>> Get(int userId);
        Task<AccountType> GetById(int id, int userId);
        Task<IEnumerable<AccountType>> GetPagination(int userId, PaginationViewModel pagination);
        Task Sort(IEnumerable<AccountType> accountTypesSorted);
        Task Update(AccountType accountType);
    }

    public class AccountTypeRepository : IAccountTypeRepository
    {
        private readonly string connectionString;

        public AccountTypeRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(AccountType accountType)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                                                "Insert_AccountType",
                                                new
                                                {
                                                    UserId = accountType.UserId,
                                                    Name = accountType.Name
                                                }, commandType: System.Data.CommandType.StoredProcedure);
            accountType.Id = id;
        }

        public async Task<bool> Exists(string name, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            var exists = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1
                                                                            FROM AccountType
                                                                            WHERE Name = @Name AND UserId = @UserId", new {name, userId});

            return exists == 1;
        }

        public async Task<IEnumerable<AccountType>> Get(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<AccountType>(@$"SELECT Id, Name, [Order] 
                                                                FROM AccountType
                                                                WHERE UserId = @UserId
                                                                ORDER BY [Order]", new { userId });
        }

        public async Task<IEnumerable<AccountType>> GetPagination(int userId, PaginationViewModel pagination)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<AccountType>(@$"SELECT Id, Name, [Order] 
                                                                FROM AccountType
                                                                WHERE UserId = @UserId
                                                                ORDER BY [Order]
                                                                OFFSET {pagination.Offset} ROWS FETCH NEXT {pagination.RecordsPerPages}
                                                                ROWS ONLY", new { userId });
        }

        public async Task<int> Count(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM AccountType WHERE UserId = @UserId", new {userId });
        }

        public async Task Update(AccountType accountType)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE AccountType
                                            SET Name = @Name
                                            WHERE Id = @Id", accountType);
        }

        public async Task<AccountType> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<AccountType>(@"SELECT Id, Name, [Order]
                                                                            FROM AccountType
                                                                            WHERE Id = @Id AND UserId = @UserId", new {id, userId});
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE AccountType
                                            WHERE Id = @Id", new { id });
        }

        public async Task Sort(IEnumerable<AccountType> accountTypesSorted)
        {
            var query = "UPDATE AccountType SET [Order] = @Order WHERE Id = @Id";
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(query, accountTypesSorted);
        }
    }
}
