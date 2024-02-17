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

        public Handler(TheatersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            IQueryable<Showing> getShowings() => _dbContext.Showings.Where(m => m.Id == request.ShowingId);

            if (_dbContext.Database.IsRelational())
            {
                await getShowings().ExecuteDeleteAsync(cancellationToken);
            }
            else
            {
                _dbContext.RemoveRange(getShowings());
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

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