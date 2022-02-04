using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<UserNxenesi> _userManager;
        private readonly SignInManager<UserNxenesi> _signInManager;
        private readonly TokenService _tokenService;
        public AccountController(UserManager<UserNxenesi> userManager,
         SignInManager<UserNxenesi> signInManager, TokenService tokenService)
        {
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;

        }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null) return Unauthorized();

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (result.Succeeded)
        {
            return CreateUserObject(user);
        }

        return Unauthorized();
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto){
        if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
        {
            return BadRequest("Email is taken");
        }

        if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
        {
            return BadRequest("Username is taken");
        }

        var user = new UserNxenesi{
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            UserName = registerDto.Username,
            Emri = registerDto.Emri,
            Mbiemri = registerDto.Mbiemri,
            Datelindja = registerDto.Datelindja,
            Rruga = registerDto.Rruga,
            Qyteti = registerDto.Qyteti,
            NumriKontaktues = registerDto.NumriKontaktues,
            Roli = 0
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if(result.Succeeded){
           return CreateUserObject(user);
        }

        return BadRequest("Problem while registering");
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser(){
        var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

        return CreateUserObject(user);
    }

    private UserDto CreateUserObject(UserNxenesi user){
            return new UserDto{
                DisplayName = user.DisplayName,
                Image = null,
                Token = _tokenService.CreateToken(user),
                Username = user.UserName,
                Emri = user.Emri,
                Mbiemri = user.Mbiemri,
                Rruga = user.Rruga,
                Qyteti = user.Qyteti,
                Datelindja = user.Datelindja,
                NumriKontaktues = user.NumriKontaktues,
                Roli = user.Roli,
                Email = user.Email
            };
    }
}
}