﻿using API.DTOs.Account;
using API.HelperModels;
using API.Models;
using API.Queries;
using API.Repositories;
using API.Services;
using MediatR;
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
        private readonly JWTService _jwtService;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public AccountController(JWTService jwtService,
            SignInManager<User> signInManager,
            IUserRepository userRepository,
            IMediator mediator)
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            var query = new GetAllUsersQuery(pageNumber, pageSize);
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("users/{code}")]
        public async Task<IActionResult> GetUserByCode(int code)
        {
            var response = await _mediator.Send(new GetUserByCodeQuery(code));
            if (response == null) return NotFound();
            return Ok(response);
        }

        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefreshUserToken()
        {
            // Get the logged-in user's email from the claim
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var query = new RefreshUserTokenQuery(email);
            var userDto = await _mediator.Send(query);

            return userDto;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginRequest loginRequest)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginRequest.UserName);
            if (user == null)
                return Unauthorized("Invalid username or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid username or password");

            return CreateApplicationUserDto(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationRequest registrationRequest)
        {
            if (await _userRepository.CheckEmailExistsAsync(registrationRequest.Email))
                return BadRequest($"{registrationRequest.Email} already Exists!");

            var lastUserCode = await _userRepository.GetLastUserCodeAsync();
            int newCode = lastUserCode + 1;

            var userToAdd = new User(newCode,
                                     registrationRequest.FirstName.ToLower(),
                                     registrationRequest.LastName.ToLower(),
                                     registrationRequest.Email.ToLower(),
                                     registrationRequest.Email.ToLower(),
                                     true);

            var result = await _userRepository.CreateUserAsync(userToAdd, registrationRequest.Password);
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
                JWT = _jwtService.CreateJWT(user),
            };
        }
        #endregion
    }
}
