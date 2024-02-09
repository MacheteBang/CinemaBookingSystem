namespace CinemaBooking.Movies;

public sealed record MovieError(string Code, string? Message = null) : Error(Code, Message)
{
    public static implicit operator Result<ICollection<Movie>>(MovieError error) => Result<ICollection<Movie>>.Failure<ICollection<Movie>>(error);
    public static implicit operator Result<Movie>(MovieError error) => Result<Movie>.Failure<Movie>(error);
}

public static class MovieErrors
{
    public static MovieError Validation(string error) => new("Movie.Validation", error);
    public static readonly MovieError NotFound = new("Movie.NotFound", "No movies found.");
}