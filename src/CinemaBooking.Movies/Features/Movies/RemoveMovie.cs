namespace CinemaBooking.Movies.Features.Movies;

public static class RemoveMovie
{
    public class Command : IRequest<Result>
    {
        public required Guid Id { get; set; }
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
            IQueryable<Movie> getMovies() => _dbContext.Movies.Where(m => m.Id == request.Id);

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

    public record Request(Guid Id);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("movies/{id:guid}", async ([AsParameters] Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand());
            if (result.IsFailure) return result.Error.ToResult();

            return Results.NoContent();
        })
        .WithName(nameof(RemoveMovieEndpoint))
        .WithTags("Movies");
    }
}

public static class RemoveMovieMapper
{
    public static RemoveMovie.Command ToCommand(this RemoveMovieEndpoint.Request request)
    {
        return new()
        {
            Id = request.Id
        };
    }
}