using BudgetManagement.Models;
using BudgetManagement.ViewModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BudgetManagement.Services
{
    public interface IAccountRepository
    {
        Task Create(Account account);
        Task Delete(int id);
        Task<IEnumerable<Account>> Find(int userId);
        Task<Account> GetById(int id, int userId);
        Task Update(AccountCreateViewModel account);
    }

    public class AccountRepository : IAccountRepository
    {
        public string connectionString { get; set; }
        public AccountRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Account account)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                @"INSERT INTO Accounts (Name, AccountTypeId, Balance, Description)
                VALUES (@Name, @AccountTypeId, @Balance, @Description);
                SELECT Scope_Identity();", account);

            account.Id = id;
        }

        public async Task<IEnumerable<Account>> Find(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Account>(@"
                        SELECT Accounts.Id, Accounts.Name, Accounts.Balance, AccountType.Name as AccountType
                        FROM Accounts
                        INNER JOIN AccountType
                        ON AccountType.Id = Accounts.AccountTypeId
                        WHERE AccountType.UserId = @UserId
                        ORDER BY AccountType.[Order];", new {userId});
        }

        public async Task<Account> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Account>(
                @"SELECT Accounts.Id, Accounts.Name, Accounts.Balance, Accounts.Description, Accounts.AccountTypeId
                    FROM Accounts
                    INNER JOIN AccountType
                    ON AccountType.Id = Accounts.AccountTypeId
                    WHERE AccountType.UserId = @UserId AND Accounts.id = @Id
                    ORDER BY AccountType.[Order]", new {id, userId});
        }

        public async Task Update(AccountCreateViewModel account)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                @"UPDATE Accounts
                SET Name = @Name, Balance = @Balance, Description = @Description, AccountTypeId = @AccountTypeId
                WHERE Id = @Id;", account);
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE Accounts WHERE Id = @Id", new {id});
        }
    }
}
