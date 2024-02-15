namespace CinemaBooking.Movies.Features.Movies;

public sealed record MovieError(string Code, IEnumerable<string>? Messages = null) : Error(Code, Messages)
{
    public static class Codes
    {
        public const string Invalid = "Movie.Invalid";
        public const string NotFound = "Movie.NotFound";
    }

    public static MovieError Validation(IEnumerable<string> errors) => new(Codes.Invalid, errors);
    public static readonly MovieError NotFound = new(Codes.NotFound, ["No movies found."]);

    public static implicit operator Result(MovieError error) => Result.Failure(error);
    public static implicit operator Result<Movie>(MovieError error) => Result.Failure<Movie>(error);
    public static implicit operator Result<ICollection<Movie>>(MovieError error) => Result.Failure<ICollection<Movie>>(error);
}