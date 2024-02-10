namespace CinemaBooking.Movies.Features.Movies;

public record GetMovieRequest(Guid Id);

public static class GetMovie
{
    public class Query : IRequest<IResult>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, IResult>
    {
        private readonly MoviesDbContext _dbContext;

        public Handler(MoviesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IResult> Handle(Query request, CancellationToken cancellationToken)
        {
            var movie = await _dbContext.Movies
                .Where(m => m.Id == request.Id)
                .Select(m => m.ToResponse())
                .FirstOrDefaultAsync(cancellationToken);

            if (movie is null) return Results.NotFound(MovieErrors.NotFound);

            return await Task.FromResult(Results.Ok(movie));
        }
    }
}

public class GetMovieEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("movies/{id:guid}", async ([AsParameters] GetMovieRequest request, ISender sender) =>
        {
            GetMovie.Query query = new() { Id = request.Id };
            return await sender.Send(query);
        })
        .WithName(nameof(GetMovie))
        .WithTags("Movies");
    }
}