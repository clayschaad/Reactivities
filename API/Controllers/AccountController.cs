using System.Security.Claims;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly TokenService tokenSevice;

        public AccountController(UserManager<AppUser> userManager, TokenService tokenSevice)
        {
            this.userManager = userManager;
            this.tokenSevice = tokenSevice;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) 
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized();
            }

            var success = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!success) 
            {
                return Unauthorized();
            }

            return CreateUserDto(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await userManager.Users.AnyAsync(u => u.UserName == registerDto.Username))
            {
                return BadRequest($"Username '{registerDto.Username}' is already taken");
            };

            if (await userManager.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest($"Email '{registerDto.Email}' is already taken");
            };

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Username
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return CreateUserDto(user);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            return CreateUserDto(user);
        }

        
        private UserDto CreateUserDto(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = null,
                Token = tokenSevice.CreateToken(user),
                Username = user.UserName
            };
        }
    }
}