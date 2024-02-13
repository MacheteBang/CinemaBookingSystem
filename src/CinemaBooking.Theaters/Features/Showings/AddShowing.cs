namespace CinemaBooking.Theaters.Features.Showings;

public static class AddShowing
{
    public class Command : IRequest<Result<Guid>>
    {
        public required Guid TheaterId { get; set; }
        public required Guid MovieId { get; set; }
        public required DateTime Showtime { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.TheaterId).NotEmpty();
            RuleFor(c => c.MovieId).NotEmpty();
            RuleFor(c => c.Showtime).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<Guid>>
    {
        public readonly IValidator<Command> _validator;
        public readonly TheatersDbContext _dbContext;

        public Handler(IValidator<Command> validator, TheatersDbContext dbContext)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ShowingErrors.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var theater = await _dbContext.Theaters.SingleOrDefaultAsync(t => t.Id == request.TheaterId, cancellationToken);
            if (theater is null) return ShowingErrors.InvalidTheather;

            Showing showing = new()
            {
                Id = Guid.NewGuid(),
                MovieId = request.MovieId,
                TheaterId = request.TheaterId,
                Showtime = request.Showtime,
                Seats = theater.SeatingArrangement.CreateSeats()
            };

            await _dbContext.Showings.AddAsync(showing, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return showing.Id;
        }
    }
}

public class AddShowingEndpoint : IEndpoint
{
    public record Request(Guid MovieId, Guid TheaterId, DateTime ShowTime);
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("showings", async ([AsParameters] Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand());

            return result.IsSuccess ? Results.CreatedAtRoute(nameof(GetShowingEndpoint), new { Id = result.Value }, new { Id = result.Value })
                : result.Error.Code switch
                {
                    ShowingErrors.Codes.InvalidTheater => Results.BadRequest(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(AddShowing))
        .WithTags("Showings");
    }
}

public static class AddShowingMapper
{
    public static AddShowing.Command ToCommand(this AddShowingEndpoint.Request request)
    {
        return new()
        {
            TheaterId = request.TheaterId,
            MovieId = request.MovieId,
            Showtime = request.ShowTime
        };
    }
}