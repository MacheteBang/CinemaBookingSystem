namespace CinemaBooking.Movies.Features.Movies;

public static class GetMovies
{
    public class Query : IRequest<Result<List<Movie>>> { }

    internal sealed class Handler : IRequestHandler<Query, Result<List<Movie>>>
    {
        private readonly MoviesDbContext _dbContext;

        public Handler(MoviesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<Movie>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var movies = await _dbContext.Movies
                .ToListAsync(cancellationToken);

            return movies ?? [];
        }
    }
}

public class GetMoviesEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("movies", async (ISender sender) =>
        {
            var result = await sender.Send(new GetMovies.Query());
            if (result.IsFailure) return result.Error.ToResult();

            return Results.Ok(result.Value.Select(m => m.ToResponse()));
        })
        .WithName(nameof(GetMoviesEndpoint))
        .WithTags("Movies");
    }
}