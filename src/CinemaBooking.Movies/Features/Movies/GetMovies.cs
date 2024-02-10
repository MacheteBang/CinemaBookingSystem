namespace CinemaBooking.Movies.Features.Movies;

public static class GetMovies
{
    public class Query : IRequest<IResult> { }

    internal sealed class Handler : IRequestHandler<Query, IResult>
    {
        private readonly MoviesDbContext _dbContext;

        public Handler(MoviesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IResult> Handle(Query request, CancellationToken cancellationToken)
        {
            var movies = _dbContext.Movies
                .Select(m => m.ToResponse());

            return await Task.FromResult(Results.Ok(movies));
        }
    }
}

public class GetMoviesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("movies", async (ISender sender) =>
        {
            return await sender.Send(new GetMovies.Query());
        })
        .WithName(nameof(GetMovies));
    }
}