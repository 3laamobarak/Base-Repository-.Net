using Company.Project.Application.Contracts;
using Company.Project.Application.DTO;
using Company.Project.Application.DTO.Account;
using Company.Project.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Company.Project.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;
        private readonly IEmailService _emailservice;
        public AccountController(IAuthService authService, UserManager<ApplicationUser> userManager, IConfiguration config, IEmailService emailservice)
        {
            _authService = authService;
            this.userManager = userManager;
            this.config = config;
            _emailservice = emailservice;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var result = await _authService.RegisterAsync(model);
            
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            return Ok(result);
        }
        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.GetTokenAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            if(!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }

            return Ok(result);
        }
        
        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRole model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.AddRoleAsync(model);
            
            if (!string.IsNullOrEmpty(result) && result.Contains("Something went wrong"))
                return BadRequest(result);
            
            return Ok(result);
        }
        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime()
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

        }
        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await _authService.RefreshTokenAsync(refreshToken);
            if(!result.IsAuthenticated)
                return BadRequest(result);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(result);


        }


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO cpDTo)
        {
            if (ModelState.IsValid)
            {
                var company = await userManager.FindByEmailAsync(cpDTo.Email);
                if (company == null)
                {
                    return NotFound("Company not found");
                }

                var result = await userManager.ChangePasswordAsync(company, cpDTo.CurrentPassword, cpDTo.NewPassword);
                if (result.Succeeded)
                {
                    return Ok("Password changed successfully");
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("request-reset-password")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] EmailDTO emailDto)
        {
            var company = await userManager.FindByEmailAsync(emailDto.To);
            if (company == null)
            {
                return NotFound("Company not found");
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(company);
            var resetLink = $"{config["FrontendUrl"]}/reset-password?email={company.Email}&token={Uri.EscapeDataString(token)}";

            await _emailservice.SendEmailAsync(new EmailDTO
            {
                To = emailDto.To,
                Subject = "Password Reset",
                Body = $"Click the link to reset your password: <a href='{resetLink}'>Reset Password</a>"
            });
            return Ok("Password reset link sent to your email.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
        {
            var company = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (company == null)
            {
                return NotFound("Company not found");
            }
            var result = await userManager.ResetPasswordAsync(company, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password reset successfully");
            }
            return BadRequest(result.Errors);
        }
        
        
    }
}
