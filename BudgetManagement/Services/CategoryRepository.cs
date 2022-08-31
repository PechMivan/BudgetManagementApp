using BudgetManagement.Models;
using BudgetManagement.Models.ViewModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BudgetManagement.Services
{
    public interface ICategoryRepository
    {
        Task Create(Category category);
        Task Delete(int id);
        Task<IEnumerable<Category>> Get(int userId, PaginationViewModel pagination);
        Task<Category> GetById(int id, int userId);
        Task Update(Category category);
        Task<IEnumerable<Category>> Get(int userId, OperationType operationTypeId);
        Task<int> Count(int userId);
    }

    public class CategoryRepository : ICategoryRepository
    {
        private readonly string connectionString;

        public CategoryRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        async Task ICategoryRepository.Create(Category category)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                @"INSERT INTO Categories (Name, OperationTypeId, UserId)
                VALUES (@Name, @OperationTypeId, @UserId);
                SELECT SCOPE_IDENTITY()", category);

            category.Id = id;
        }

        async Task<IEnumerable<Category>> ICategoryRepository.Get(int userId, PaginationViewModel pagination)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(@$"SELECT * FROM Categories 
                                                            WHERE UserId = @UserId
                                                            ORDER BY Name
                                                            OFFSET {pagination.Offset} ROWS FETCH NEXT {pagination.RecordsPerPages}
                                                            ROWS ONLY", new { userId });
        }

        public async Task<int> Count(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Categories WHERE UserId = @UserId", new { userId });
        }

        async Task<IEnumerable<Category>> ICategoryRepository.Get(int userId, OperationType operationTypeId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(@"SELECT * FROM Categories 
                                                        WHERE UserId = @UserId 
                                                        AND OperationTypeId = @OperationTypeId", new { userId, operationTypeId });
        }

        async Task<Category> ICategoryRepository.GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Category>("SELECT * FROM Categories WHERE Id = @Id AND UserId = @UserId", new { id, userId });
        }

        async Task ICategoryRepository.Update(Category category)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Categories
                                            SET Name = @Name, OperationTypeId = @OperationTypeId
                                            WHERE Id = @Id", category);
        }

        async Task ICategoryRepository.Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE Categories WHERE Id = @Id", new { id });
        }
    }
}
