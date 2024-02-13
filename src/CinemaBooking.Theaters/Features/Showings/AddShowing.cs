

namespace CinemaBooking.Theaters.Features.Showings;

public static class AddShowing
{
    public class Command : IRequest<Result<Guid>>
    {
        public required Guid TheaterId { get; set; }
        public required Guid MovieId { get; set; }
        public required DateTime Showtime { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<Guid>>
    {
        public readonly TheatersDbContext _dbContext;

        public Handler(TheatersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var theater = await _dbContext.Theaters.SingleOrDefaultAsync(t => t.Id == request.TheaterId, cancellationToken);
            if (theater is null) return ShowingErrors.InvalidTheather;

            Showing showing = new()
            {
                Id = Guid.NewGuid(),
                MovieId = request.MovieId,
                TheaterId = request.TheaterId,
                Showtime = request.Showtime,
                Seats = theater.SeatingArrangement.CreateSeats()
            };

            await _dbContext.Showings.AddAsync(showing, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return showing.Id;
        }
    }
}

public class AddShowingEndpoint : IEndpoint
{
    public record Request(Guid MovieId, Guid TheaterId, DateTime ShowTime);
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("showings", async (Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand());

            return result.IsSuccess ? Results.Ok(result.Value)
                : result.Error.Code switch
                {
                    ShowingErrors.Codes.InvalidTheater => Results.BadRequest(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(AddShowing))
        .WithTags("Showings");
    }
}

public static class AddShowingMapper
{
    public static AddShowing.Command ToCommand(this AddShowingEndpoint.Request request)
    {
        return new()
        {
            TheaterId = request.TheaterId,
            MovieId = request.MovieId,
            Showtime = request.ShowTime
        };
    }
}