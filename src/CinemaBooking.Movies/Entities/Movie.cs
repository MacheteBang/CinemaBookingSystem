namespace CinemaBooking.Movies.Entities;

public class Movie
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public ICollection<Genre> Genres { get; set; } = [];
}