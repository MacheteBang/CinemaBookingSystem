namespace CinemaBooking.Theaters.Errors;

public sealed record ReservationError(string Code, IEnumerable<string>? Messages = null) : Error(Code, Messages)
{
    public static class Codes
    {
        public const string Invalid = "Seat.Invalid";
        public const string NotFound = "Seat.NotFound";
    }

    public static ReservationError Validation(IEnumerable<string> errors) => new(Codes.Invalid, errors);
    public static readonly ReservationError NotFound = new(Codes.NotFound, ["Reservation not found."]);

    public static implicit operator Result(ReservationError error) => Result.Failure(error);
    public static implicit operator Result<Reservation>(ReservationError error) => Result.Failure<Reservation>(error);
    public static implicit operator Result<List<Reservation>>(ReservationError error) => Result.Failure<List<Reservation>>(error);
}