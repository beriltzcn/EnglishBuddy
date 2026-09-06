using EnglishBuddy.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnglishBuddy.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatService _chat;
        public ChatController(ChatService chat)
        {
            _chat = chat;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(string message)
        {
            if(string.IsNullOrWhiteSpace(message))
            {
                return RedirectToAction(nameof(Index));
            }

            var reply = await _chat.GetReplyAsync(message);

            ViewBag.UserMessage = message;
            ViewBag.Reply = reply;
            return View("Index");

        }
    }
}