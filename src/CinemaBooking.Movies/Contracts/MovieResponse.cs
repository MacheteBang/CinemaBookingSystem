namespace CinemaBooking.Movies.Contracts;

public class MovieResponse
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }

    public string? Description { get; set; }

    public TimeSpan? Duration { get; set; }

    public List<Genre>? Genres { get; set; }
}