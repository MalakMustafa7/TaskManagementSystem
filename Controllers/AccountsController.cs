using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagement.Api.DTOs;
using TeamTaskManagement.Api.Errors;
using TeamTaskManagement.Core.Entities.Identity;
using TeamTaskManagement.Core.Service;

namespace TeamTaskManagement.Api.Controllers
{

    public class AccountsController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountsController(UserManager<AppUser> userManager,
                                  ITokenService tokenService,
                                  SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>>Register(RegisterDTO model)
        {
            if (!CheckEmailExists(model.Email).Result.Value)
                return BadRequest(new ApiResponse(400, "This Email Is Already Exist"));
            var user = new AppUser
            {
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Email.Split('@')[0],     
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400, "SomeThing Went Wrong"));
            else
            {
                await _userManager.AddToRoleAsync(user, "Member");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            var returnedUser = new UserDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = role,
                Token = await _tokenService.GetTokenAsync(user, _userManager)

            };
            return Ok(returnedUser);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>>Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null) return NotFound(new ApiResponse(404, "The Email Is InCorrect"));
            var chechedPassword = await _signInManager.CheckPasswordSignInAsync(user, model.Password,false);
            if(!chechedPassword.Succeeded)
                return Unauthorized();
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            var returnedUser = new UserDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = role,
                Token = await _tokenService.GetTokenAsync(user, _userManager)

            };
            return Ok(returnedUser);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<UserDTO>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            var returnedUser = new UserDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = role,
                Token = await _tokenService.GetTokenAsync(user, _userManager)

            };
            return Ok(returnedUser);

        }

        [HttpGet("EmailExist")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return _userManager.FindByEmailAsync(email) is not null;
        }
    }
}
