namespace CinemaBooking.Movies.Features.Movies;

public static class RemoveMovie
{
    public class Command : IRequest<Result>
    {
        public required Guid MovieId { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        public readonly MoviesDbContext _dbContext;

        public Handler(MoviesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            IQueryable<Movie> getMovies() => _dbContext.Movies.Where(m => m.Id == request.MovieId);

            if (_dbContext.Database.IsRelational())
            {
                await getMovies().ExecuteDeleteAsync(cancellationToken);
            }
            else
            {
                _dbContext.RemoveRange(getMovies());
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }
    }
}

public class RemoveMovieEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("movies/{movieId:guid}", async (Guid movieId, ISender sender) =>
        {
            var result = await sender.Send(new RemoveMovie.Command { MovieId = movieId });
            if (result.IsFailure) return result.Error.ToResult();

            return Results.NoContent();
        })
        .WithName(nameof(RemoveMovieEndpoint))
        .WithTags("Movies");
    }
}