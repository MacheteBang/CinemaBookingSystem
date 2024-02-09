namespace CinemaBooking.Movies.Features.Movies;

public record GetMovieRequest(Guid Id);

public static class GetMovie
{
    public class Query : IRequest<Result<Movie>>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Movie>>
    {
        private readonly MoviesDbContext _dbContext;

        public Handler(MoviesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Movie>> Handle(Query request, CancellationToken cancellationToken)
        {
            var movie = await _dbContext.Movies
                .Where(m => m.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (movie is null) return MovieErrors.NotFound;

            return await Task.FromResult(movie);
        }
    }
}

public class GetMovieEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("movies/{id}", async ([AsParameters] GetMovieRequest request, ISender sender) =>
        {
            GetMovie.Query query = new() { Id = request.Id };

            var result = await sender.Send(query);

            if (result.IsFailure) return Results.NotFound(result.Error);

            return Results.Ok(result.Value);
        })
        .WithName(nameof(GetMovie));
    }
}