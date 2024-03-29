namespace CinemaBooking.Movies.Features.Movies;

public static class UpdateMovie
{
    public class Command : IRequest<Result>
    {
        public required Guid MovieId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TimeSpan? Duration { get; set; }
        public List<Genre>? Genres { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.MovieId).NotEmpty();
            RuleFor(c => c.Title).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly IValidator<Command> _validator;
        private readonly MoviesDbContext _dbContext;

        public Handler(IValidator<Command> validator, MoviesDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return MovieError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            Movie movie = new()
            {
                Id = request.MovieId,
                Title = request.Title,
                Description = request.Description,
                Duration = request.Duration,
                Genres = request.Genres
            };

            _dbContext.Movies.Update(movie);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

public class UpdateMovieEndpoint : IEndpoint
{
    public record UpdateMovieRequest(
        string Title,
        string? Description,
        TimeSpan? Duration,
        List<Genre>? Genres
    );

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("movies/{movieId:guid}", async (Guid movieId, UpdateMovieRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand(movieId));
            if (result.IsFailure) return result.Error.ToResult();

            return Results.Accepted();
        })
        .WithName(nameof(UpdateMovieEndpoint))
        .WithTags("Movies");
    }
}

public static class UpdateMovieMapper
{
    public static UpdateMovie.Command ToCommand(this UpdateMovieEndpoint.UpdateMovieRequest request, Guid movieId)
    {
        return new()
        {
            MovieId = movieId,
            Title = request.Title,
            Description = request.Description,
            Duration = request.Duration,
            Genres = request.Genres
        };
    }
}