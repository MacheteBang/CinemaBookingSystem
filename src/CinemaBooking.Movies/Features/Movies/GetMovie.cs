namespace CinemaBooking.Movies.Features.Movies;

public static class GetMovie
{
    public class Query : IRequest<Result<Movie>>
    {
        public required Guid MovieId { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(c => c.MovieId).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Movie>>
    {
        private readonly IValidator<Query> _validator;
        private readonly MoviesDbContext _dbContext;

        public Handler(IValidator<Query> validator, MoviesDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async Task<Result<Movie>> Handle(Query request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return MovieError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var movie = await _dbContext.Movies
                .Where(m => m.Id == request.MovieId)
                .SingleOrDefaultAsync(cancellationToken);

            if (movie is null) return MovieError.NotFound;

            return movie;
        }
    }
}

public class GetMovieEndpoint : IEndpoint
{
    public record Request(Guid MovieId);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("movies/{movieId:guid}", async (Guid movieId, ISender sender) =>
        {
            var result = await sender.Send(new GetMovie.Query() { MovieId = movieId });
            if (result.IsFailure) return result.Error.ToResult();

            return Results.Ok(result.Value.ToResponse());
        })
        .WithName(nameof(GetMovieEndpoint))
        .WithTags("Movies");
    }
}