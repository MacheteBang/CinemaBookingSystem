namespace CinemaBooking.Movies.Features.Movies;

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

            return movie;
        }
    }
}

public class GetMovieEndpoint : IEndpoint
{
    public record Request(Guid Id);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("movies/{id:guid}", async ([AsParameters] Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToQuery());

            return result.IsSuccess ? Results.Ok(result.Value.ToResponse())
                : result.Error.Code switch
                {
                    MovieErrors.Codes.NotFound => Results.NotFound(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(GetMovieEndpoint))
        .WithTags("Movies");
    }
}

public static class GetMovieMapper
{
    public static GetMovie.Query ToQuery(this GetMovieEndpoint.Request request)
    {
        return new()
        {
            Id = request.Id
        };
    }
}