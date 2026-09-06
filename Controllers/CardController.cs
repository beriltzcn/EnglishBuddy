using EnglishBuddy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishBuddy.Controllers
{
    public class CardController : Controller
    {
        private readonly AppDbContext _db;

        public CardController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var cards = await _db.WordCard
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
            return View(cards);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(WordCard card)
        {
            if (string.IsNullOrWhiteSpace(card.Word) || string.IsNullOrWhiteSpace(card.Meaning))
            {
                TempData["Error"] = "Word and Meaning are required.";
                return View(card);
            }

            _db.WordCard.Add(card);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Review()
        {
            var total = await _db.WordCard.CountAsync();
            if (total == 0)
            {
                return View();
            }
            var skip = Random.Shared.Next(total);
            var card = await _db.WordCard
                .OrderBy(x => x.Id)
                .Skip(skip)
                .FirstOrDefaultAsync();

            return View(card);
        }

        [HttpPost]
        public async Task<IActionResult> Known(int id)
        {
            var card = await _db.WordCard.FindAsync(id);
            if(card == null)
            {
                card.ReviewCount++;
                card.CorrectCount++;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Review));
        }

        [HttpPost]
        public async Task<IActionResult> Unknown (int id)
        {
            var card = await _db.WordCard.FindAsync(id);
            if(card != null)
            {
                card.ReviewCount++;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Review));
        }

    }
}
