using Microsoft.AspNetCore.Http;

namespace CinemaBooking.Common;

public partial record Error(string Code, IEnumerable<string>? Messages = null)
{
    public static readonly Error None = new(string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", ["The specified result value is null."]);
    public static readonly Error ConditionNotMet = new("Error.ConditionNotMet", ["The specified condition was not met."]);

    public IResult ToResult()
    {
        if (Code.Contains("NotFound", StringComparison.InvariantCultureIgnoreCase)) return Results.NotFound(Messages);

        return Results.BadRequest(Messages);
    }
}