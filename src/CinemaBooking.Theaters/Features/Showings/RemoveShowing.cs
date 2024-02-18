namespace CinemaBooking.Theaters.Features.Showings;

public static class RemoveShowing
{
    public class Command : IRequest<Result>
    {
        public required Guid ShowingId { get; set; }
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
            // Explicit validation not used as it is done when getting the Showing.
            // If this changes to not get the Showing first, then validation needs
            // to be introduced.

            var showingResult = await _mediator
                .Send(new GetShowing.Query { ShowingId = request.ShowingId }, cancellationToken);
            if (showingResult.IsFailure) return showingResult.Error;
            var showing = showingResult.Value;

            _dbContext.Remove(showing);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

public class RemoveShowingEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("showings/{showingId:guid}", async (Guid showingId, ISender sender) =>
        {
            var result = await sender.Send(new RemoveShowing.Command { ShowingId = showingId });
            if (result.IsFailure) return result.Error.ToResult();

            return Results.NoContent();
        })
        .WithName(nameof(RemoveShowingEndpoint))
        .WithTags("Showings");
    }
}