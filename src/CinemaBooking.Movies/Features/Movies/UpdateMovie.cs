namespace CinemaBooking.Movies.Features.Movies;

public static class UpdateMovie
{
    public class Command : IRequest<Result>
    {
        public required Guid Id { get; set; }
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
                return MovieErrors.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            Movie movie = new()
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Duration = request.Duration,
                Genres = request.Genres?.Count == 0
                    ? null
                    : request.Genres?.Select(g => (Genre)Enum.Parse(typeof(Genre), g)).ToList()
            };

            _dbContext.Movies.Update(movie);
            _dbContext.SaveChanges();

            return await Task.FromResult(Result.Success());
        }
    }
}

public class UpdateMovieEndpoint : IEndpoint
{
    public record Request(
    string Title,
    string? Description = null,
    TimeSpan? Duration = null,
    ICollection<string>? Genres = null
);


    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("movies/{id:guid}", async (Guid id, Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand(id));

            return result.IsSuccess ? Results.Accepted()
                : result.Error.Code switch
                {
                    MovieErrors.Codes.Invalid => Results.BadRequest(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(UpdateMovieEndpoint))
        .WithTags("Movies");
    }
}

public static class UpdateMovieMapper
{
    public static UpdateMovie.Command ToCommand(this UpdateMovieEndpoint.Request request, Guid id)
    {
        return new()
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            Duration = request.Duration,
            Genres = request.Genres
        };
    }
}