namespace CinemaBooking.Theaters.Features.Showings;

public static class GetShowing
{
    public class Query : IRequest<Result<Showing>>
    {
        public Guid ShowingId { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Showing>>
    {
        private readonly TheatersDbContext _dbContext;

        public Handler(TheatersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Showing>> Handle(Query request, CancellationToken cancellationToken)
        {
            var showing = await _dbContext.Showings
                .Where(t => t.Id == request.ShowingId)
                .SingleOrDefaultAsync(cancellationToken);

            if (showing is null) return ShowingError.NotFound;

            return showing;
        }
    }
}

public class GetShowingEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("showings/{showingId:guid}", async (Guid showingId, ISender sender) =>
        {
            var result = await sender.Send(new GetShowing.Query { ShowingId = showingId });
            if (result.IsFailure) return result.Error.ToResult();

            return Results.Ok(result.Value.ToResponse());
        })
        .WithName(nameof(GetShowingEndpoint))
        .WithTags("Showings");
    }
}