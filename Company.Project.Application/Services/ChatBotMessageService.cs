using System.Text;
using System.Text.Json;
using Company.Project.Application.Contracts;
using Company.Project.Domain.Interfaces;
using Company.Project.Domain.Models;

namespace Company.Project.Application.Services
{
    public class ChatBotMessageService : IChatBotMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;
        
        public ChatBotMessageService(IUnitOfWork unitOfWork, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
        }
        
        public async Task<(ChatBotMessages userMsg, ChatBotMessages botMsg)> SendMessageAsync(string userId, string message)
        {
            var userMessage = new ChatBotMessages
            {
                Message = message,
                Sender = "User",
                UserId = userId,
            };
            await _unitOfWork.ChatBotMessages.AddAsync(userMessage);
            string botReply;
            try
            {
                botReply = await GetBotReplyAsync(message, userId);
            }
            catch (Exception)
            {
                botReply = "Sorry, I'm having trouble responding right now.";
            }
            var botMessage = new ChatBotMessages
            {
                Message = botReply,
                Sender = "Bot",
                UserId = userId,
            };
            await _unitOfWork.ChatBotMessages.AddAsync(botMessage);
            await _unitOfWork.CompleteAsync();
            return (userMessage, botMessage);
        }

        public async Task<IEnumerable<ChatBotMessages>> GetMessagesByUserAsync(string userId)
        {
            return await _unitOfWork.ChatBotMessages.GetByExpressionAsync(msg => msg.UserId == userId);
        }

        public async Task<IEnumerable<ChatBotMessages>> GetAllMessagesAsync()
        {
            return await _unitOfWork.ChatBotMessages.GetAllAsync();
        }
        public async Task DeleteMessagesByUserAsync(string userId)
        {
            var messages = await _unitOfWork.ChatBotMessages.GetByExpressionAsync(msg => msg.UserId == userId);
            foreach (var msg in messages)
            {
                await _unitOfWork.ChatBotMessages.DeleteAsync(msg);
            }
            await _unitOfWork.CompleteAsync();
        }
        private async Task<string> GetBotReplyAsync(string message, string userId)
        {
            try
            {
                var history = await _unitOfWork.ChatBotMessages.GetUserMessagesAsync(userId);
               
                var messages = new List<object>
                {
                    new { role = "system", content = "You are a helpful chatbot. Be friendly and helpful. Always address the user by their name if available." }
                };
                foreach (var msg in history)
                {
                    messages.Add(new
                    {
                        role = msg.Sender.ToLower() == "User" ? "user" : "assistant", 
                        content = msg.Message
                    });
                }
                messages.Add(new {role = "user", content = message});
                
                var requestDate = new
                {
                    model = "meta-llama/llama-3.1-8b-instruct",
                    messages = messages.ToArray(),
                    max_tokens = 200
                };
                var json = JsonSerializer.Serialize(requestDate);
                var response = await _httpClient.PostAsync("chat/completions",
                    new StringContent(json, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return "Sorry, I'm having trouble responding right now.";
                }

                var result = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(result);
                var choices = doc.RootElement.GetProperty("choices");
                if (choices.GetArrayLength() > 0)
                {
                    return choices[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString() ?? "I'm sorry, I couldn't generate a response. Please try again.";
                }
                return "I'm sorry, I couldn't generate a response. Please try again.";
            }
            catch (Exception ex)
            {
                return "I'm having trouble processing your request right now. Please try again later.";
            }
        }
    }
}
