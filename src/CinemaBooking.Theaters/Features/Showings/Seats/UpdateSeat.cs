namespace CinemaBooking.Showings.Features.Showings.Seats;

public class UpdateSeat
{
    public class Command : IRequest<Result<Seat>>
    {
        public required Guid ShowingId { get; set; }
        public required Guid Id { get; set; }
        public required Seat.OccupancyAction Action { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.ShowingId).NotEmpty();
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.Action).IsInEnum();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<Seat>>
    {
        private readonly IValidator<Command> _validator;
        private readonly TheatersDbContext _dbContext;
        private readonly IMediator _mediator;

        public Handler(IValidator<Command> validator, TheatersDbContext dbContext, IMediator mediator)
        {
            _validator = validator;
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<Result<Seat>> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return SeatError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var showing = await _mediator.Send(new GetShowing.Query { Id = request.ShowingId }, cancellationToken);
            if (showing.IsFailure) return SeatError.Validation(showing.Error.Messages ?? []);

            var seat = showing.Value.Seats.SingleOrDefault(s => s.Id == request.Id);
            if (seat is null) return SeatError.NotFound;

            var actionResult = request.Action switch
            {
                Seat.OccupancyAction.Reserve => seat.Reserve(),
                Seat.OccupancyAction.Release => seat.Release(),
                Seat.OccupancyAction.Confirm => seat.Confirm(),
                _ => SeatError.InvalidAction
            };

            if (actionResult.IsFailure) return SeatError.OccupancyChange;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return seat;
        }

    }
}

public class UpdateSeatEndpoint : IEndpoint
{
    public record Request(Seat.OccupancyAction Action);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch(
            "showings/{showingId:guid}/seats/{id:guid}",
            async (Guid showingId, Guid id, Request request, ISender sender) =>
            {
                var result = await sender.Send(request.ToCommand(showingId, id));

                return result.IsSuccess ? Results.Ok(result.Value)
                    : result.Error.Code switch
                    {
                        SeatError.Codes.NotFound => Results.NotFound(result.Error.Messages),
                        _ => Results.BadRequest(result.Error.Messages)
                    };
            });
    }
}

public static class UpdateSeatMapper
{
    public static UpdateSeat.Command ToCommand(this UpdateSeatEndpoint.Request request, Guid showingid, Guid id)
    {
        return new()
        {
            ShowingId = showingid,
            Id = id,
            Action = request.Action
        };
    }
}