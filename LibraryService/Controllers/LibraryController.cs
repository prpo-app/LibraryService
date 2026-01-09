using LibraryService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace LibraryService.Controllers
{
    [Route("/library")]
    [ApiController]
    [Authorize]
    public class LibraryController : ControllerBase
    {
        private readonly ILogger<LibraryController> _logger;
        private readonly IConfiguration _config;

        private AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public LibraryController(ILogger<LibraryController> logger, IConfiguration config, AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _config = config;
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Returns a list of books in the user's library.
        /// </summary>
        /// <param name="offset">Starting index (default 0).</param>
        /// <param name="limit">Maximum number of items returned (default 5).</param>
        /// <param name="status">Optional book status filter.</param>
        /// <response code="200">Returns the user's library.</response>
        /// <response code="400">Invalid parameters.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">Access denied.</response>
        [HttpGet]
        public IActionResult GetLibrary(
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 5,
            [FromQuery] BookStatus? status = null)
        {
            if (offset < 0 || limit <= 0)
                return BadRequest("Invalid pagination parameters.");

            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            var query = _context.MyLibrary
                .Where(ub => ub.User_id == userId)
                .AsQueryable();

            if (status.HasValue)
            {
                var statusString = status.Value switch
                {
                    BookStatus.WantToRead => "Want to read",
                    BookStatus.CurrentlyReading => "Currently reading",
                    BookStatus.Read => "Read",
                    _ => null
                };

                if (statusString == null)
                    return BadRequest("Invalid book status.");

                query = query.Where(b => b.Status == statusString);
            }
                var books = query
                    .OrderBy(b => b.Book_id)
                    .Skip(offset)
                    .Take(limit)
                    .ToList();

            return Ok(books);
        }


        /// <summary>
        /// Adds a book to the user's library
        /// </summary>
        /// <param name="bookId">book identificator</param>
        /// <param name="status">book status (read, want to read, currently reading)</param>
        /// <response code="201">Book added to library.</response>
        /// <response code="409">Book is already in the library.</response>
        /// <response code="400">Invalid parameters.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">Access denied.</response>
        /// <response code="404">Book doesn't exist.</response>
        /// <response code="503">BookService is unavailable.</response>
        [HttpPost]
        public async Task<IActionResult> AddBook(int bookId, BookStatus status)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
            );

            bool exists = _context.MyLibrary.Any(ub =>
                ub.User_id == userId && ub.Book_id == bookId);

            if (exists)
                return Conflict("Book already in your library.");

            var client = _httpClientFactory.CreateClient("BookService");
            var response = await client.GetAsync($"/book/{bookId}");
       

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound("Book doesn't exist.");
            }

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(503, "BookService unavailable.");
            }
            
            var statusString = status switch
            {
                BookStatus.WantToRead => "Want to read",
                BookStatus.CurrentlyReading => "Currently reading",
                BookStatus.Read => "Read",
                _ => throw new ArgumentOutOfRangeException()
            };

            var entry = new MyLibrary
            {
                User_id = userId,
                Book_id = bookId,
                Status = statusString
            };

            _context.MyLibrary.Add(entry);
            _context.SaveChanges();

            return Created();
        }

        /// <summary>
        /// Removes a book from the user's library.
        /// </summary>
        /// <param name="id">Id of removed book.</param>
        /// <response code="204">Book removed from library.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="403">Access denied.</response>
        /// <response code="404">Book not found in library.</response>
        [HttpDelete("{bookId:int}")]
        public IActionResult DeleteBook(int bookId)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var entry = _context.MyLibrary
                .SingleOrDefault(e =>
                    e.User_id == userId &&
                    e.Book_id == bookId);

            if (entry == null)
                return NotFound("Book not in your library.");

            _context.MyLibrary.Remove(entry);
            _context.SaveChanges();

            return NoContent();
        }

        public enum BookStatus
        {
            WantToRead,
            CurrentlyReading, 
            Read
        }
    }
}
