namespace CinemaBooking.Theaters.Features.Theaters;

public static class TheaterErrors
{
    public static class Codes
    {
        public const string Invalid = "Theater.Invalid";
        public const string NotFound = "Theater.NotFound";
    }

    public static TheaterError Validation(IEnumerable<string> errors) => new(Codes.Invalid, errors);
    public static readonly TheaterError NotFound = new(Codes.NotFound, ["No theaters found."]);
}

public sealed record TheaterError(string Code, IEnumerable<string>? Messages = null) : Error(Code, Messages)
{
    public static implicit operator Result(TheaterError error) => Result.Failure(error);
    public static implicit operator Result<Theater>(TheaterError error) => Result.Failure<Theater>(error);
    public static implicit operator Result<List<Theater>>(TheaterError error) => Result.Failure<List<Theater>>(error);
}