namespace CinemaBooking.Movies.Features.Movies;

public static class MovieErrors
{
    public const string InvalidEnumTemplate = "'{PropertyValue}' is not a valid Genre.";

    public static Error Validation(IEnumerable<string> errors) => new("Movie.Validation", errors);
    public static readonly Error NotFound = new("Movie.NotFound", ["No movies found."]);
}