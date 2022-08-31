using BudgetManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BudgetManagement.Services
{
    public interface ITransactionRepository
    {
        Task Create(Transaction transaction);
        Task Delete(int id);
        Task<IEnumerable<Transaction>> GetByAccountId(TransactionPerAccount model);
        Task<Transaction> GetById(int id, int userId);
        Task<IEnumerable<Transaction>> GetByUserId(TransactionPerUser model);
        Task<IEnumerable<MonthlyReportModel>> GetReportsByMonth(int userId, int year);
        Task<IEnumerable<WeeklyReportModel>> GetReportsByWeek(TransactionPerUser model);
        Task Update(Transaction transaction, int previousAccountId, decimal previousAmount);
    }
    public class TransactionRepository : ITransactionRepository
    {
        private readonly string connectionString;

        public TransactionRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Transaction transaction)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Insert_Transaction",
                new
                {
                    transaction.UserId,
                    transaction.TransactionDate,
                    transaction.Amount,
                    transaction.AccountId,
                    transaction.CategoryId,
                    transaction.Note
                }, commandType: System.Data.CommandType.StoredProcedure);

            transaction.Id = id;
        }

        public async Task Update(Transaction transaction, int previousAccountId, decimal previousAmount)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Update_Transaction",
                new
                {
                    transaction.Id,
                    transaction.TransactionDate,
                    transaction.Amount,
                    transaction.AccountId,
                    transaction.CategoryId,
                    transaction.Note,
                    previousAccountId,
                    previousAmount
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaction> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaction>(
                @"SELECT Transactions.*, cat.OperationTypeId
                FROM Transactions
                INNER JOIN Categories cat
                ON cat.Id = Transactions.CategoryId
                WHERE Transactions.Id = @Id AND Transactions.UserId = @UserId;", new { id, userId });
        }

        public async Task<IEnumerable<Transaction>> GetByAccountId(TransactionPerAccount model)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaction>(
                @"SELECT t.Id, t.TransactionDate, t.Amount, acc.Name as Account, cat.Name as Category, cat.OperationTypeId
                    FROM Transactions t
                    INNER JOIN Categories cat ON cat.Id = t.CategoryId
                    INNER JOIN Accounts acc ON acc.Id = t.AccountId
                    WHERE t.UserId = @UserId AND t.AccountId = @AccountId
                    AND t.TransactionDate BETWEEN @StartDate AND @EndDate", model);
        }

        public async Task<IEnumerable<Transaction>> GetByUserId(TransactionPerUser model)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaction>(
                @"SELECT t.Id, t.TransactionDate, t.Amount, acc.Name as Account, cat.Name as Category, cat.OperationTypeId, t.Note
                    FROM Transactions t
                    INNER JOIN Categories cat ON cat.Id = t.CategoryId
                    INNER JOIN Accounts acc ON acc.Id = t.AccountId
                    WHERE t.UserId = @UserId
                    AND t.TransactionDate BETWEEN @StartDate AND @EndDate
                    ORDER BY t.TransactionDate DESC", model);
        }

        public async Task<IEnumerable<WeeklyReportModel>> GetReportsByWeek(TransactionPerUser model)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<WeeklyReportModel>(
                @"SELECT DATEDIFF(d, @StartDate, TransactionDate) / 7 as Week,
                    SUM(Amount) as Amount, cat.OperationTypeId
                    FROM Transactions
                    INNER JOIN Categories cat ON cat.Id = Transactions.CategoryId
                    WHERE Transactions.UserId = @UserId AND
                    Transactions.TransactionDate BETWEEN @StartDate AND @EndDate
                    GROUP BY DATEDIFF(d, @StartDate, TransactionDate) / 7, cat.OperationTypeId;", model);
        }

        public async Task<IEnumerable<MonthlyReportModel>> GetReportsByMonth(int userId, int year)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<MonthlyReportModel>(
                @"SELECT MONTH(Transactions.TransactionDate) as Month,
                    SUM(Amount) as Amount, cat.OperationTypeId
                    FROM Transactions
                    INNER JOIN Categories cat ON Cat.Id = Transactions.CategoryId
                    WHERE Transactions.UserId = @UserId AND YEAR(Transactions.TransactionDate) = @Year
                    GROUP BY MONTH(Transactions.TransactionDate), cat.OperationTypeId;", new { userId, year });
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Delete_Transaction", new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
