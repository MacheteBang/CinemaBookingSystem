namespace CinemaBooking.Movies.Features.Movies;

public record AddMovieRequest(string Title, string? Description = null, TimeSpan? Duration = null, ICollection<Genre>? Genres = null);

public static class AddMovie
{
    public class Command : IRequest<Result<Guid>>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TimeSpan? Duration { get; set; }
        public ICollection<Genre>? Genres { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Title).NotEmpty();
            RuleForEach(c => c.Genres).IsInEnum();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<Guid>>
    {
        private readonly IValidator<Command> _validator;
        private readonly MoviesDbContext _dbContext;

        public Handler(IValidator<Command> validator, MoviesDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return MovieErrors.Validation(validationResult.ToString());
            }

            Movie movie = new()
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Duration = request.Duration,
                Genres = request.Genres
            };

            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();

            return await Task.FromResult(movie.Id);
        }
    }
}

public class AddMovieEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("movies", async (AddMovieRequest request, ISender sender) =>
        {
            AddMovie.Command command = new()
            {
                Title = request.Title,
                Description = request.Description,
                Duration = request.Duration,
                Genres = request.Genres
            };

            var result = await sender.Send(command);

            if (result.IsFailure) return Results.BadRequest(result.Error);

            return Results.CreatedAtRoute(nameof(GetMovie), new { id = result.Value });
        })
        .WithName(nameof(AddMovie));
    }
}