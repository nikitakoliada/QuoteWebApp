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

namespace QuotesWebApp.Controllers
{
    public class QuotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Quotes
        public async Task<IActionResult> Index(string currentFilter, string searchString, int? pagenumber)
        {
            if (searchString != null)
            {
                pagenumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var quotes = from s in _context.Quote
                           select s;
            int pageSize = 10;
            return View(await PaginatedList<Quote>.CreateAsync(quotes.AsNoTracking(), pagenumber ?? 1, pageSize));

        }
        //int pageNumber = 1)

        //return View(await PagedList<Quote>.CreateAsync(_context.Quote, pageNumber, 20));

        // GET: Quotes/SearchForm
        public async Task<IActionResult> SearchForm()
        {
            return View();
        }
        //Post: Quotes/SearchResults
        public async Task<IActionResult> SearchResults(String QuotePhrase, String Author, String Tags, String Category)
        {
            var quotes = from j in _context.Quote select j;

            quotes = quotes.Where(j => j.QuoteText.Contains(QuotePhrase) || j.Author.Contains(Author) || j.Tags.Contains(Tags) || j.Category.Contains(Category));
            int pageSize = 10;
            int pagenumber = 1;
            return View("Index", await PaginatedList<Quote>.CreateAsync(quotes.AsNoTracking(), pagenumber, pageSize));
        }

        [Authorize]
        public async void Favourite(Quote quote)
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
        public async Task<IActionResult> Create([Bind("Id,QuoteText,Author,Tags,Category")] Quote quote)
        {
            {
                _context.Add(quote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(quote);
        }

        // GET: Quotes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
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
            if (id != quote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
                return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
        }

        private bool QuoteExists(int id)
        {
          return _context.Quote.Any(e => e.Id == id);
        }
    }
}
