namespace CinemaBooking.Theaters.Features.Theaters;

public static class GetTheaters
{
    public class Query : IRequest<Result<List<Theater>>> { }

    internal sealed class Handler : IRequestHandler<Query, Result<List<Theater>>>
    {
        private readonly TheatersDbContext _dbContext;

        public Handler(TheatersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<Theater>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var theaters = await _dbContext.Theaters
                .ToListAsync(cancellationToken);

            if (theaters is null) return TheaterError.NotFound;

            return theaters;
        }
    }
}

public class GetTheatersEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("theaters", async (ISender sender) =>
        {
            var result = await sender.Send(new GetTheaters.Query());
            if (result.IsFailure) return result.Error.ToResult();

            return Results.Ok(result.Value.Select(t => t.ToResponse()));
        })
        .WithName(nameof(GetTheatersEndpoint))
        .WithTags("Theaters");
    }
}