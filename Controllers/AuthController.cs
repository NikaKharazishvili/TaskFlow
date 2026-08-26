using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Controllers;

[ApiController, Route("api/[controller]")]
public class AuthController : ControllerBase
{
    readonly UserManager<User> userManager; // Handles creating users, hashing passwords, checking uniqueness
    readonly SignInManager<User> signInManager; // Handles verifying login credentials
    readonly ITokenService tokenService;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new User { UserName = dto.Email, Email = dto.Email };

        var result = await userManager.CreateAsync(user, dto.Password); // Automatically hashes password and checks email/username uniqueness
        if (!result.Succeeded) return BadRequest(result.Errors); // e.g. "email already taken", weak password, etc.

        return Ok(new { token = tokenService.CreateToken(user) });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null) return Unauthorized("Invalid email or password");

        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded) return Unauthorized("Invalid email or password");

        return Ok(new { token = tokenService.CreateToken(user) });
    }
}