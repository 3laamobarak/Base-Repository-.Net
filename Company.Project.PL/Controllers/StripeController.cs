using Company.Project.Application.Contracts;
using Company.Project.Application.DTO.PaymentDTO;
using Company.Project.Domain.Interfaces;
using Company.Project.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Company.Project.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StripeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripeService _stripeService;

        public StripeController(IStripeService stripeService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _stripeService = stripeService;
        }

        [HttpPost("process-payment")]
        public async Task<ActionResult<PaymentResultDto>> ProcessPayment([FromBody] CreatePaymentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("user not authenticated.");
                var result = await _stripeService.ProcessPaymentAsync(request, userId);
                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing the payment.", Details = ex.Message });
            }
        }


        [HttpPost("process-saved-payment")]
        public async Task<IActionResult> ProcessSavedPayment(int savedPaymentMethodId, decimal amount, string currency)
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var result = await _stripeService.ProcessSavedPaymentAsync(savedPaymentMethodId, amount, currency, userId);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("save-payment-method")]
        public async Task<IActionResult> SavePaymentMethod([FromBody] SavePaymentMethodRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var result = await _stripeService.SavePaymentMethodAsync(request, userId);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }
        [HttpPost("refund-payment/{transactionId}")]
        public async Task<IActionResult> RefundPayment(string transactionId, decimal? amount = null)
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var success = await _stripeService.RefundPaymentAsync(transactionId, amount);
            if (success)
                return Ok(new { Message = "Refund successful." });

            return BadRequest(new { Message = "Refund failed." });
        }

        [HttpGet("payment-methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var result = await _stripeService.GetPaymentMethods(userId);
            return Ok(result);
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransaction()
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");
            var result = await _stripeService.GetTransactions(userId);
            return Ok(result);
        }
    }
}
