namespace CinemaBooking.Theaters.Features.Theaters;

public static class RemoveTheater
{
    public class Command : IRequest<Result>
    {
        public required Guid TheaterId { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        public readonly TheatersDbContext _dbContext;
        public readonly IMediator _mediator;

        public Handler(TheatersDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var theaterResult = await _mediator
                .Send(new GetTheater.Query { TheaterId = request.TheaterId }, cancellationToken);
            if (theaterResult.IsFailure) return theaterResult.Error;
            var theater = theaterResult.Value;

            _dbContext.Remove(theater);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

public class RemoveTheaterEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("theaters/{theaterId:guid}", async (Guid theaterId, ISender sender) =>
        {
            var result = await sender.Send(new RemoveTheater.Command { TheaterId = theaterId });
            if (result.IsFailure) return result.Error.ToResult();

            return Results.NoContent();
        })
        .WithName(nameof(RemoveTheaterEndpoint))
        .WithTags("Theaters");
    }
}