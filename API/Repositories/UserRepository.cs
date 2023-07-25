using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<User>> GetPagedUsersAsync(int pageNumber, int pageSize)
        {
            int itemsToSkip = (pageNumber - 1) * pageSize;
            return await _userManager.Users
                .Skip(itemsToSkip)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _userManager.Users.CountAsync();
        }
        public async Task<User> GetUserByCodeAsync(int code)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Code == code);
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<int> GetLastUserCodeAsync()
        {
            return await _userManager.Users
                .OrderByDescending(u => u.Code)
                .Select(u => u.Code)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) throw new Exception("User creation failed");

            return result;
        }
    }
}
