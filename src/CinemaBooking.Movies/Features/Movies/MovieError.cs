namespace CinemaBooking.Movies.Features.Movies;

public static class MovieErrors
{
    public static class Codes
    {
        public const string Invalid = "Movie.Invalid";
        public const string NotFound = "Movie.NotFound";
    }


    public const string InvalidEnumTemplate = "'{PropertyValue}' is not a valid Genre.";

    public static MovieError Validation(IEnumerable<string> errors) => new(Codes.Invalid, errors);
    public static readonly MovieError NotFound = new(Codes.NotFound, ["No movies found."]);
}

public sealed record MovieError(string Code, IEnumerable<string>? Messages = null) : Error(Code, Messages)
{
    public static implicit operator Result<Movie>(MovieError error) => Result.Failure<Movie>(error);
}