namespace CinemaBooking.Movies.Options;

public class MoviesOptions
{
    public MoviesDbProvider DbProvider { get; set; } = MoviesDbProvider.InMemory;
}