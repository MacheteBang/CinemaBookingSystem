namespace CinemaBooking.Movies.Features.Movies;

public static class GetMovies
{
    public class Query : IRequest<Result<ICollection<Movie>>> { }

    internal sealed class Handler : IRequestHandler<Query, Result<ICollection<Movie>>>
    {
        private readonly MoviesDbContext _dbContext;

        public Handler(MoviesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<ICollection<Movie>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var movies = await _dbContext.Movies
                .ToListAsync(cancellationToken);

            return movies;
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

            return result.IsSuccess ? Results.Ok(result.Value.Select(m => m.ToResponse()))
                : result.Error.Code switch
                {
                    MovieError.Codes.NotFound => Results.NotFound(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(GetMoviesEndpoint))
        .WithTags("Movies");
    }
}