namespace CinemaBooking.Theaters.Errors;

public sealed record ShowingError(string Code, IEnumerable<string>? Messages = null) : Error(Code, Messages)
{
    public static class Codes
    {
        public const string Invalid = "Showing.Invalid";
        public const string NotFound = "Showing.NotFound";
        public const string InvalidTheater = "Showing.InvalidTheater";
    }

    public static ShowingError Validation(IEnumerable<string> errors) => new(Codes.Invalid, errors);
    public static readonly ShowingError NotFound = new(Codes.NotFound, ["No showtimes found."]);
    public static readonly ShowingError InvalidTheather = new(Codes.InvalidTheater, ["No theaters found with that Id"]);

    public static implicit operator Result(ShowingError error) => Result.Failure(error);
    public static implicit operator Result<Showing>(ShowingError error) => Result.Failure<Showing>(error);
    public static implicit operator Result<List<Showing>>(ShowingError error) => Result.Failure<List<Showing>>(error);
}