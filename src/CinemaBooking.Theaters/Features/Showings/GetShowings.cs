namespace CinemaBooking.Theaters.Features.Showings;

public static class GetShowings
{
    public class Query : IRequest<Result<List<Showing>>> { }

    internal sealed class Handler : IRequestHandler<Query, Result<List<Showing>>>
    {
        private readonly TheatersDbContext _dbContext;

        public Handler(TheatersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<Showing>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var showings = await _dbContext.Showings
                .ToListAsync(cancellationToken);

            if (showings is null) return ShowingError.NotFound;

            return showings;
        }
    }
}

public class GetShowingsEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("showings", async (ISender sender) =>
        {
            var result = await sender.Send(new GetShowings.Query());

            return result.IsSuccess ? Results.Ok(result.Value.Select(t => t.ToResponse()))
                : result.Error.Code switch
                {
                    ShowingError.Codes.NotFound => Results.NotFound(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(GetShowingsEndpoint))
        .WithTags("Showings");
    }
}