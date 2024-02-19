namespace CinemaBooking.Theaters.Errors;

public static class ReservationError
{
    public static class Codes
    {
        public const string Invalid = "Reservation.Invalid";
        public const string NotFound = "Reservation.NotFound";
    }

    public static Error Validation(IEnumerable<string> errors) => new(Codes.Invalid, errors);
    public static readonly Error NotFound = new(Codes.NotFound, ["No Reservation exists with that Id."]);
}