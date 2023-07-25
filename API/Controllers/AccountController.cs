using API.DTOs.Account;
using API.Models;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTService _jWTService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public AccountController(JWTService jwtService,
            SignInManager<User> signInManager,
            IUserRepository userRepository)
        {
            _jWTService = jwtService;
            _signInManager = signInManager;
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            int itemsToSkip = (pageNumber - 1) * pageSize;
            int totalUsersCount = await _userRepository.GetTotalUsersCountAsync();

            var users = await _userRepository.GetPagedUsersAsync(pageNumber, pageSize);

            var response = new
            {
                TotalCount = totalUsersCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = users
            };

            return Ok(response);
        }

        [Authorize]
        [HttpGet("users/{code}")]
        public async Task<IActionResult> GetUserByCode(int code)
        {
            var user = await _userRepository.GetUserByCodeAsync(code);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefreshUserToken()
        {
            var user = await _userRepository.GetUserByEmailAsync(User.FindFirst(ClaimTypes.Email)?.Value);
            return CreateApplicationUserDto(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.UserName);
            if (user == null)
                return Unauthorized("Invalid username or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid username or password");

            return CreateApplicationUserDto(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (await _userRepository.CheckEmailExistsAsync(registerDto.Email))
                return BadRequest($"{registerDto.Email} already Exists!");

            var lastUserCode = await _userRepository.GetLastUserCodeAsync();
            int newCode = lastUserCode + 1;

            var userToAdd = new User
            {
                FirstName = registerDto.FirstName.ToLower(),
                LastName = registerDto.LastName.ToLower(),
                UserName = registerDto.Email.ToLower(),
                Email = registerDto.Email.ToLower(),
                Code = newCode,
                EmailConfirmed = true,
                LockoutEnabled = false
            };

            var result = await _userRepository.CreateUserAsync(userToAdd, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Account created successfully");
        }

        #region Helper Methods
        private UserDto CreateApplicationUserDto(User user)
        {
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = _jWTService.CreateJWT(user),
            };
        }
        #endregion
    }
}
