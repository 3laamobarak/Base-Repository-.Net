using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Company.Project.Application.Contracts;
using Company.Project.Application.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtSecurityTokenHandler = System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler;

namespace Company.Project.PL.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class AccountController : Controller
    {
        private readonly UserManager<Domain.Models.Company> userManager;
        private readonly IConfiguration config;
        private readonly IOTPService _otpService;
         private readonly IEmailService _emailservice;

        public AccountController
            (UserManager<Domain.Models.Company> userManager,IConfiguration config, IOTPService otpService , IEmailService emailservice)
        {
            this.userManager = userManager;
            this._otpService = otpService;
            this.config = config;
            this._emailservice = emailservice;
        }
        //api/account/register :post
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCompanyDto userDto)
        {
            if (ModelState.IsValid)
            {
                var existingUser = 
                    await userManager.FindByEmailAsync(userDto.Email);
                if (existingUser != null)
                {
                    return BadRequest("Email already exists");
                }
                var isOtpValid = 
                    await _otpService.ValidateOTPAsync(userDto.Email, userDto.otpCode);
                if (!isOtpValid)
                {
                    return BadRequest("Invalid OTP code");
                }
                Domain.Models.Company company = new Domain.Models.Company()
                {
                    UserName = userDto.Email,
                    Email = userDto.Email,
                    EnglishName = userDto.EnglishName,
                    ArabicName = userDto.ArabicName,
                    Phone = userDto.Phone,
                    Logo = userDto.Logo,
                    websiteURL = userDto.WebsiteURL
                };
                //create account in db
                IdentityResult result= 
                    await  userManager.CreateAsync(company,userDto.Password);       
                if(result.Succeeded)
                {
                    var token = GenerateJwtToken(company);
                    return Ok(new{Token = token});
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("Login")]//api/account/login (username /passwor)
        public async Task<IActionResult> Login(LoginCompanyDTO userDto)
        {
            if (ModelState.IsValid){
                Domain.Models.Company? company=
                    await  userManager.FindByEmailAsync(userDto.Email);
                if (company != null)
                {
                    bool found=await userManager.CheckPasswordAsync(company, userDto.Password);
                    if (found)
                    {
                        var token = GenerateJwtToken(company);
                        return Ok(new { Token = token });
                    }
                }
                return Unauthorized("Invalid account");
            }
            return BadRequest(ModelState);
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
        
        private string GenerateJwtToken(Domain.Models.Company company)
        // private IActionResult GenerateJwtToken(Domain.Models.Company company)
        {
            List<Claim> myclaims = new List<Claim>();
            myclaims.Add(new Claim(ClaimTypes.Email, company.Email));
            myclaims.Add(new Claim(ClaimTypes.NameIdentifier, company.Id));
            myclaims.Add(new Claim("EnglishName", company.EnglishName));
            myclaims.Add(new Claim("ArabicName", company.ArabicName));

            //myclaims.Add(new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()));

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecritKey"]));
            var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["JWT:Issuer"],
                audience: config["JWT:Audience"],
                claims: myclaims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);

            // return Ok(new
            // {
            //     token = new JwtSecurityTokenHandler().WriteToken(token),
            //     expired = token.ValidTo
            // });
        }
        
        
    }
}
