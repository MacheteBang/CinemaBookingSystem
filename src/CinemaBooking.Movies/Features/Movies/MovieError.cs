namespace CinemaBooking.Movies.Features.Movies;

public sealed record MovieError(string Code, string? Message = null) : Error(Code, Message)
{
    public static implicit operator Result<ICollection<MovieResponse>>(MovieError error) => Result<ICollection<MovieResponse>>.Failure<ICollection<MovieResponse>>(error);
    public static implicit operator Result<MovieResponse>(MovieError error) => Result<MovieResponse>.Failure<MovieResponse>(error);
}

public static class MovieErrors
{
    public static MovieError Validation(string error) => new("Movie.Validation", error);
    public static readonly MovieError NotFound = new("Movie.NotFound", "No movies found.");
}