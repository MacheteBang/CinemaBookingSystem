namespace CinemaBooking.Theaters.Errors;

public static class ShowingError
{
    public static class Codes
    {
        public const string Invalid = "Showing.Invalid";
        public const string NotFound = "Showing.NotFound";
    }

    public static Error Validation(IEnumerable<string> errors) => new(Codes.Invalid, errors);
    public static readonly Error NotFound = new(Codes.NotFound, ["No Showing exists with that Id."]);
}