namespace CinemaBooking.Movies.Features.Movies;

public static class AddMovie
{
    public class Command : IRequest<Result<Guid>>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TimeSpan? Duration { get; set; }
        public ICollection<string>? Genres { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Title).NotEmpty();
            RuleForEach(c => c.Genres).IsEnumName(typeof(Genre)).WithMessage(MovieErrors.InvalidEnumTemplate);
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
                return MovieErrors.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            Movie movie = new()
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Duration = request.Duration,
                Genres = request.Genres?.Count == 0
                    ? null
                    : request.Genres?.Select(g => (Genre)Enum.Parse(typeof(Genre), g)).ToList()
            };

            await _dbContext.Movies.AddAsync(movie, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return movie.Id;
        }
    }
}

public class AddMovieEndpoint : IEndpoint
{
    public record Request(string Title, string? Description = null, TimeSpan? Duration = null, ICollection<string>? Genres = null);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("movies", async (Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand());

            return result.IsSuccess ? Results.CreatedAtRoute(nameof(GetMovieEndpoint), new { Id = result.Value }, new { Id = result.Value })
                : result.Error.Code switch
                {
                    MovieErrors.Codes.Invalid => Results.BadRequest(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(AddMovieEndpoint))
        .WithTags("Movies");
    }
}

public static class AddMovieMapper
{
    public static AddMovie.Command ToCommand(this AddMovieEndpoint.Request request)
    {
        return new()
        {
            Title = request.Title,
            Description = request.Description,
            Duration = request.Duration,
            Genres = request.Genres
        };
    }
}