namespace CinemaBooking.Theaters.Features.Showings;

public static class UpdateShowing
{
    public class Command : IRequest<Result>
    {
        public required Guid ShowingId { get; set; }
        public required Guid TheaterId { get; set; }
        public required Guid MovieId { get; set; }
        public required DateTime Showtime { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.ShowingId).NotEmpty();
            RuleFor(c => c.TheaterId).NotEmpty();
            RuleFor(c => c.MovieId).NotEmpty();
            RuleFor(c => c.Showtime).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        public readonly IValidator<Command> _validator;
        private readonly TheatersDbContext _dbContext;
        private readonly IMediator _mediator;

        public Handler(IValidator<Command> validator, TheatersDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _validator = validator;
            _mediator = mediator;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ShowingError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var showingResult = await _mediator
                .Send(new GetShowing.Query { ShowingId = request.ShowingId }, cancellationToken);
            if (showingResult.IsFailure) return showingResult.Error;
            var showing = showingResult.Value;

            // Reservations associated with a Showing are purposely not updated here.
            showing.TheaterId = request.TheaterId;
            showing.MovieId = request.MovieId;
            showing.Showtime = request.Showtime;

            _dbContext.Showings.Update(showing);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

public class UpdateShowingEndpoint : IEndpoint
{
    public record Request(
        Guid TheaterId,
        Guid MovieId,
        DateTime ShowTime
    );

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("showings/{showingId:guid}", async (Guid showingId, Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand(showingId));
            if (result.IsFailure) return result.Error.ToResult();

            return Results.Accepted();
        })
        .WithName(nameof(UpdateShowingEndpoint))
        .WithTags("Showings");
    }
}

public static class UpdateShowingMapper
{
    public static UpdateShowing.Command ToCommand(this UpdateShowingEndpoint.Request request, Guid showingId)
    {
        return new()
        {
            ShowingId = showingId,
            TheaterId = request.TheaterId,
            MovieId = request.MovieId,
            Showtime = request.ShowTime
        };
    }
}