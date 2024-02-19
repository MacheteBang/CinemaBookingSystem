namespace CinemaBooking.Theaters.Errors;

public static class SeatError
{
    public static class Codes
    {
        public const string Invalid = "Seat.Invalid";
        public const string NotFound = "Seat.NotFound";
    }

    public static Error Validation(IEnumerable<string> errors) => new(Codes.Invalid, errors);
    public static readonly Error NotFound = new(Codes.NotFound, ["No Seat exists with that Id."]);
}