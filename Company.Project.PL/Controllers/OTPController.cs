using Company.Project.Application.Contracts;
using Company.Project.Application.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Company.Project.PL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OTPController : ControllerBase
    {
        private readonly IOTPService _otpService;

        public OTPController(IOTPService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendOTP([FromBody] EmailDTO emailDto)
        {
            try
            {
                var otp = await _otpService.GenerateAndSendOTPAsync(emailDto.To);
                return Ok("OTP sent successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateOTP([FromBody] OTPDTO otpDto)
        {
            var isValid = await _otpService.ValidateOTPAsync(otpDto.Email, otpDto.Code);
            return isValid ? Ok("OTP validated successfully") : BadRequest("Invalid OTP");
        }
    }
}