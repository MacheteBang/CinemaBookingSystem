namespace CinemaBooking.Theaters.Errors;

public static class SeatError
{
    public static class Codes
    {
        public const string Invalid = "Seat.Invalid";
        public const string NotFound = "Seat.NotFound";
        public const string Unavailable = "Seat.Unavailable";
    }

    public static Error Validation(IEnumerable<string> errors) => new(Codes.Invalid, errors);
    public static readonly Error NotFound = new(Codes.NotFound, ["No Seat exists with that Id."]);
    public static readonly Error Unavailable = new(Codes.Unavailable, ["Seat was found, but is unavailable to reserve."]);
}