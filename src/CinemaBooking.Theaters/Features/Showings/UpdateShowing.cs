
namespace CinemaBooking.Theaters.Features.Showings;

public static class UpdateShowing
{
    public class Command : IRequest<Result>
    {
        public required Guid Id { get; set; }
        public required Guid TheaterId { get; set; }
        public required Guid MovieId { get; set; }
        public required DateTime Showtime { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly TheatersDbContext _dbContext;

        public Handler(TheatersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            Showing? showing = await _dbContext.Showings
                .SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (showing is null) return ShowingErrors.NotFound;

            showing.TheaterId = request.TheaterId;
            showing.MovieId = request.MovieId;
            showing.Showtime = request.Showtime;

            _dbContext.Showings.Update(showing);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

public class UpdateShowingEndpoint : IEndpoint
{
    public record Request(
        Guid TheaterId,
        Guid MovieId,
        DateTime ShowTime
    );

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("showings/{id:guid}", async (Guid id, Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand(id));

            return result.IsSuccess ? Results.Accepted()
                : result.Error.Code switch
                {
                    ShowingErrors.Codes.NotFound => Results.NotFound(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(UpdateShowingEndpoint))
        .WithTags("Showings");
    }
}

public static class UpdateShowingMapper
{
    public static UpdateShowing.Command ToCommand(this UpdateShowingEndpoint.Request request, Guid id)
    {
        return new()
        {
            Id = id,
            TheaterId = request.TheaterId,
            MovieId = request.MovieId,
            Showtime = request.ShowTime
        };
    }
}