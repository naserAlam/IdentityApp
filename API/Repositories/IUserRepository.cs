using API.DTOs.Account;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<IEnumerable<User>> GetPagedUsersAsync(int pageNumber, int pageSize);
        Task<int> GetTotalUsersCountAsync();
        Task<User> GetUserByCodeAsync(int code);
        Task<User> GetUserByEmailAsync(string email);
        Task<int> GetLastUserCodeAsync();
        Task<bool> CheckEmailExistsAsync(string email);
        Task<IdentityResult> CreateUserAsync(User user, string password);
    }
}
