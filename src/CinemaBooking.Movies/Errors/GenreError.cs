namespace CinemaBooking.Movies.Errors;

public sealed record GenreError(string Code, IEnumerable<string>? Messages = null) : Error(Code, Messages)
{
    public static implicit operator Result(GenreError error) => Result.Failure(error);
    public static implicit operator Result<string>(GenreError error) => Result.Failure<string>(error);
    public static implicit operator Result<ICollection<string>>(GenreError error) => Result.Failure<ICollection<string>>(error);
}