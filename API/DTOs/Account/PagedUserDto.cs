using API.Models;

namespace API.DTOs.Account
{
    public class PagedUserDto
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
