using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuotesWebApp.Data;
using QuotesWebApp.Models;
using PagedList;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace QuotesWebApp.Controllers
{
    public class QuotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public QuotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        //GET: Quotes
        public async Task<IActionResult> Index(
        string quotePhrase,
        string author,
        string tags,
        string category,
        string? currentFilterQuotePhrase,
        string? currentFilterAuthor,
        string? currentFilterTags,
        string? currentFilterCategory,
        int? pageNumber)
        {

            if (!String.IsNullOrEmpty(quotePhrase) || !String.IsNullOrEmpty(author) || !String.IsNullOrEmpty(tags) || !String.IsNullOrEmpty(category))
            {
                pageNumber = 1;
            }
            else
            {

                quotePhrase = currentFilterQuotePhrase ?? "";
                author = currentFilterAuthor ?? "";
                tags = currentFilterTags ?? "";
                category = currentFilterCategory ?? "";
            }

            quotePhrase = quotePhrase == null ? "" : quotePhrase;
            author = author == null ? "" : author;
            tags = tags == null ? "" : tags;
            category = category == null ? "" : category;

            ViewData["currentFilterQuotePhrase"] = quotePhrase;
            ViewData["currentFilterAuthor"] = author;
            ViewData["currentFilterTags"] = tags;
            ViewData["currentFilterCategory"] = category;

            var quotes = from s in _context.Quote
                         select s;


            if (!String.IsNullOrEmpty(quotePhrase) || !String.IsNullOrEmpty(author) || !String.IsNullOrEmpty(tags) || !String.IsNullOrEmpty(category))
            {
                quotes = quotes.Where(j => j.QuoteText.Contains(quotePhrase) && j.Author.Contains(author) && j.Tags.Contains(tags) && j.Category.Contains(category));
            }

            int pageSize = 10;
            return View(await PaginatedList<Quote>.CreateAsync(quotes.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [Authorize]
        //GET: Quotes/Favourite
        public async Task<IActionResult> Favourite(int? pageNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            var userFavQuotes =  from s in _context.UserFavQuotes
                         select s;
            userFavQuotes =  userFavQuotes.Where(i => i.UserId == user.Id);

            var quotes = from s in _context.Quote
                         select s;
            List<int> idsFavQuotes = new List<int>();
            foreach (var userFavQuote in userFavQuotes)
            {
                idsFavQuotes.Add(userFavQuote.QuoteId);
            }
            quotes = quotes.Where(i => idsFavQuotes.Contains(i.Id));

            return View(await PaginatedList<Quote>.CreateAsync(quotes.AsNoTracking(), pageNumber ?? 1, 10));
        }
        [Authorize]
        //GET: Quotes/YourQoutes
        public async Task<IActionResult> OwnQuotes(int? pageNumber)
        {
            var quotes = from s in _context.Quote
                         select s;
            var user = await _userManager.GetUserAsync(User);

            quotes = quotes.Where(i => i.Email == user.Email);

            return View(await PaginatedList<Quote>.CreateAsync(quotes.AsNoTracking(), pageNumber ?? 1, 10));
        }

        // GET: Quotes/SearchForm
        public IActionResult SearchForm()
        {
            return View();
        }
        //Post: Quotes/SearchResults
        public IActionResult SearchResults(
    string quotePhrase,
    string author,
    string tags,
    string category)
        {

            return RedirectToAction("Index", new
            {
                quotePhrase = quotePhrase,
                author = author,
                tags = tags,
                category = category
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> FavouriteConfirmed(int id)
        {
            if (ModelState.IsValid)
            {
                if (id == null || _context.Quote == null)
                {
                    return NotFound();
                }

                Quote quote = await _context.Quote.FirstOrDefaultAsync(m => m.Id == id);
                if (quote == null)
                {
                    return NotFound();
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }
                try
                {
                    var userFavQuote = await _context.UserFavQuotes.FirstOrDefaultAsync(m => m.QuoteId == id && m.UserId == user.Id);

                    string msg = "";

                    if (userFavQuote != null)
                    {
                        _context.UserFavQuotes.Remove(userFavQuote);
                        await _context.SaveChangesAsync();
                        msg = "The quote was removed";

                    }
                    else
                    {
                        userFavQuote = new UserFavQuotes();
                        userFavQuote.UserId = user.Id;
                        userFavQuote.QuoteId = quote.Id;

                        _context.UserFavQuotes.Add(userFavQuote);
                        msg = "The quote was added";

                        //TODO only 
                    }
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = msg });
                }
                catch
                {
                    return Json(new { success = false, message = "An error accured" });
                }

            }
            return RedirectToAction(nameof(Index));
        }



        // GET: Quotes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Quote == null)
            {
                return NotFound();
            }

            var quote = await _context.Quote
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quote == null)
            {
                return NotFound();
            }

            return View(quote);
        }

        // GET: Quotes/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Quotes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,QuoteText,Author,Tags,Category, Email")] Quote quote)
        {
            if (ModelState.IsValid) {
                var user = await _userManager.GetUserAsync(User);
                quote.Email = user.Email;
                _context.Add(quote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(OwnQuotes));
            }
            return View(quote);
    }

        // GET: Quotes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit( int? id)
        {
            if (id == null || _context.Quote == null)
            {
                return NotFound();
            }

            var quote = await _context.Quote.FindAsync(id);
            if (quote == null)
            {
                return NotFound();
            }
            return View(quote);
        }

        // POST: Quotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,QuoteText,Author,Tags,Category")] Quote quote)
        {
            if (ModelState.IsValid)
            {

                if (id != quote.Id)
                {
                    return NotFound();
                }
                var user = await _userManager.GetUserAsync(User);
                quote.Email = user.Email;

                try
                {
                    _context.Update(quote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuoteExists(quote.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(OwnQuotes));
            }
            return View(quote);
        }

        // GET: Quotes/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Quote == null)
            {
                return NotFound();
            }

            var quote = await _context.Quote
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quote == null)
            {
                return NotFound();
            }

            return View(quote);
        }

        // POST: Quotes/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Quote == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Quote'  is null.");
            }
            var quote = await _context.Quote.FindAsync(id);
            if (quote != null)
            {
                _context.Quote.Remove(quote);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(OwnQuotes));
        }

        private bool QuoteExists(int id)
        {
          return _context.Quote.Any(e => e.Id == id);
        }
    }
}
