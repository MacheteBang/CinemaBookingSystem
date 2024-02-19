namespace CinemaBooking.Theaters.Errors;

public static class TheaterError
{
    public static class Codes
    {
        public const string Invalid = "Theater.Invalid";
        public const string NotFound = "Theater.NotFound";
    }

    public static Error Validation(IEnumerable<string> errors) => new(Codes.Invalid, errors);
    public static readonly Error NotFound = new(Codes.NotFound, ["No Theater exists with that Id."]);
}