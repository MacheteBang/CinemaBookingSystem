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
        public readonly IMediator _mediator;

        public Handler(MoviesDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            // Explicit validation not used as it is done when getting the Movie.
            // If this changes to not get the Movie first, then validation needs
            // to be introduced.

            var movieResult = await _mediator
                .Send(new GetMovie.Query { MovieId = request.MovieId }, cancellationToken);
            if (movieResult.IsFailure) return movieResult.Error;
            var movie = movieResult.Value;

            _dbContext.Remove(movie);
            await _dbContext.SaveChangesAsync(cancellationToken);

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