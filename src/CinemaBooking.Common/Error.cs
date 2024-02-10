namespace CinemaBooking.Common;

public record Error(string Code, IEnumerable<string>? Message = null)
{
    public static readonly Error None = new(string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", ["The specified result value is null."]);
    public static readonly Error ConditionNotMet = new("Error.ConditionNotMet", ["The specified condition was not met."]);
}