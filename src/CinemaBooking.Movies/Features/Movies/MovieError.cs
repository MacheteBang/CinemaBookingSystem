namespace CinemaBooking.Movies.Features.Movies;

public static class MovieErrors
{
    public static class Codes
    {
        public const string Invalid = "Movie.Invalid";
        public const string NotFound = "Movie.NotFound";
    }


    public const string InvalidEnumTemplate = "'{PropertyValue}' is not a valid Genre.";

    public static Error Validation(IEnumerable<string> errors) => new(Codes.Invalid, errors);
    public static readonly Error NotFound = new(Codes.NotFound, ["No movies found."]);

}