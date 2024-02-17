namespace CinemaBooking.Movies.Domain;

public class Movie : Entity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TimeSpan? Duration { get; set; }
    public List<Genre>? Genres { get; set; }
}