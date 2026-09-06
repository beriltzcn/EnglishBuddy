using EnglishBuddy.Models;
using EnglishBuddy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishBuddy.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatService _chat;
        private readonly AppDbContext _db;

        public ChatController(ChatService chat, AppDbContext db)
        {
            _chat = chat;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var history = await _db.ChatMessages
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
            return View(history);
        }

        [HttpPost]
        public async Task<IActionResult> Send(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return RedirectToAction(nameof(Index));
            }

            var history = await _db.ChatMessages
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();

            var userMsg = new ChatMessage { Role = "user", Content = message };
            history.Add(userMsg);
            _db.ChatMessages.Add(userMsg);
            await _db.SaveChangesAsync();

            var reply = await _chat.GetReplyAsync(history);
            _db.ChatMessages.Add(new ChatMessage { Role = "assistant", Content = reply });
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}