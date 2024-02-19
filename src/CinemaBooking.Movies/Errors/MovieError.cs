namespace CinemaBooking.Movies.Features.Movies;

public static class MovieError
{
    public static class Codes
    {
        public const string Invalid = "Movie.Invalid";
        public const string NotFound = "Movie.NotFound";
    }

    public static Error Validation(IEnumerable<string> errors) => new(Codes.Invalid, errors);
    public static readonly Error NotFound = new(Codes.NotFound, ["No Movie exists with that Id."]);
}