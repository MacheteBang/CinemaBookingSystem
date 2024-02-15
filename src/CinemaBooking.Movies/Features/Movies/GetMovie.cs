namespace CinemaBooking.Movies.Features.Movies;

public static class GetMovie
{
    public class Query : IRequest<Result<Movie>>
    {
        public required Guid Id { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(c => c.Id).NotEmpty();
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
                .Where(m => m.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (movie is null) return MovieError.NotFound;

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
                    MovieError.Codes.NotFound => Results.NotFound(result.Error.Messages),
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