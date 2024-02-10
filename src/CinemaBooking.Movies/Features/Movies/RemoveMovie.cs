namespace CinemaBooking.Movies.Features.Movies;

public record RemoveMovieRequest(Guid Id);

public static class RemoveMovie
{
    public class Command : IRequest<IResult>
    {
        public required Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Command, IResult>
    {
        public readonly MoviesDbContext _dbContext;

        public Handler(MoviesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
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

            return Results.NoContent();
        }
    }
}

public class RemoveMovieEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("movies/{id:guid}", async ([AsParameters] RemoveMovieRequest request, ISender sender) =>
        {
            RemoveMovie.Command command = new() { Id = request.Id };
            return await sender.Send(command);
        });
    }
}