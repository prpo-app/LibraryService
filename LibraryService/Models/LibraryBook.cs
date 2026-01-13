namespace LibraryService.Models
{
    public class LibraryBook
    {
        public int BookId { get; set; }
        public string Title { get; set; } = default!;
        public string Author { get; set; } = default!;
        public string Genre { get; set; } = default!;
        public string Status { get; set; } = default!;
    }
}
