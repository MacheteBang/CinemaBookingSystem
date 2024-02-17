namespace CinemaBooking.Theaters.Errors;

public sealed record SeatError(string Code, IEnumerable<string>? Messages = null) : Error(Code, Messages)
{
    public static class Codes
    {
        public const string Invalid = "Seat.Invalid";
        public const string NotFound = "Seat.NotFound";
        public const string TheaterNotFound = "Seat.TheaterNotFound";
    }

    public static SeatError Validation(IEnumerable<string> errors) => new(Codes.Invalid, errors);
    public static readonly SeatError NotFound = new(Codes.NotFound, ["No seats found."]);
    public static readonly SeatError TheaterNotFound = new(Codes.TheaterNotFound, ["No theaters found with that Id."]);

    public static implicit operator Result(SeatError error) => Result.Failure(error);
    public static implicit operator Result<Seat>(SeatError error) => Result.Failure<Seat>(error);
    public static implicit operator Result<List<Seat>>(SeatError error) => Result.Failure<List<Seat>>(error);
}