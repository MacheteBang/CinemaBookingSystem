using CinemaBooking.Theaters.Features.Theaters;

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
        public readonly IMediator _mediator;

        public Handler(IValidator<Command> validator, TheatersDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _validator = validator;
            _mediator = mediator;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ShowingError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var theaterResult = await _mediator
                .Send(new GetTheater.Query { TheaterId = request.TheaterId }, cancellationToken);
            if (theaterResult.IsFailure) return theaterResult.Error;
            var theater = theaterResult.Value;

            // TODO: Validate the MovieID from another service

            Showing showing = new()
            {
                Id = Guid.NewGuid(),
                MovieId = request.MovieId,
                TheaterId = request.TheaterId,
                Showtime = request.Showtime
            };

            await _dbContext.Showings.AddAsync(showing, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return showing.Id;
        }
    }
}

public class AddShowingEndpoint : IEndpoint
{
    public record AddShowingRequest(Guid MovieId, Guid TheaterId, DateTime ShowTime);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("showings", async (AddShowingRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand());
            if (result.IsFailure) return result.Error.ToResult();

            return Results.CreatedAtRoute(
                nameof(GetShowingEndpoint),
                new { ShowingId = result.Value },
                new { Id = result.Value }
            );
        })
        .WithName(nameof(AddShowingEndpoint))
        .WithTags("Showings");
    }
}

public static class AddShowingMapper
{
    public static AddShowing.Command ToCommand(this AddShowingEndpoint.AddShowingRequest request)
    {
        return new()
        {
            TheaterId = request.TheaterId,
            MovieId = request.MovieId,
            Showtime = request.ShowTime
        };
    }
}