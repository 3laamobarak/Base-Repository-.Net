using System.Security.Claims;
using Company.Project.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Company.Project.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatBotMessageService _chatBotMessageService;
        public ChatbotController(IChatBotMessageService chatBotMessageService)
        {
            _chatBotMessageService = chatBotMessageService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] string message)
        {
            try
            {
                var userId =User.FindFirst("uid")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not Authenticated.");
                }
                if(string.IsNullOrWhiteSpace(message))
                {
                    return BadRequest("Message cannot be empty.");
                }
                var (userMsg, botMsg) = await _chatBotMessageService.SendMessageAsync(userId, message);
                return Ok(new
                {
                    UserMessage = userMsg.Message, 
                    BotMessage = botMsg.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("History")]
        public async Task<IActionResult> GetHistory()
        {
            try
            {
                var userId =User.FindFirst("uid")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not Authenticated.");
                }
                var messages = await _chatBotMessageService.GetMessagesByUserAsync(userId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("History")]
        public async Task<IActionResult> DeleteHistory()
        {
            try
            {
                var userId =User.FindFirst("uid")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not Authenticated.");
                }
                await _chatBotMessageService.DeleteMessagesByUserAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        
    }
}