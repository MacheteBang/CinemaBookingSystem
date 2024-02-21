using CinemaBooking.Theaters.Features.Theaters.Reservations;
using CinemaBooking.Theaters.Features.Theaters.Seats;

namespace CinemaBooking.Theaters.Features.Showings.Reservations;

public static class AddReservation
{
    public class Command : IRequest<Result<Guid>>
    {
        public required Guid ShowingId { get; set; }
        public required Guid SeatId { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.ShowingId).NotEmpty();
            RuleFor(c => c.SeatId).NotEmpty();
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
                return ReservationError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var showingQuery = new GetShowing.Query() { ShowingId = request.ShowingId };
            var showingResult = await _mediator.Send(showingQuery, cancellationToken);
            if (showingResult.IsFailure) return showingResult.Error;
            var showing = showingResult.Value;

            var seatQuery = new GetSeat.Query() { TheaterId = showing.TheaterId, SeatId = request.SeatId };
            var seatResult = await _mediator.Send(seatQuery, cancellationToken);
            if (seatResult.IsFailure) return seatResult.Error;
            var seat = seatResult.Value;

            var anyBlockingReservations = (showing.Reservations ?? [])
                .Any(r => r.SeatId == request.SeatId && r.IsActive());

            if (anyBlockingReservations) return SeatError.Unavailable;

            Reservation reservation = new()
            {
                Id = Guid.NewGuid(),
                SeatId = request.SeatId,
                State = ReservationState.Pending,
                PendingExpiresOn = DateTime.UtcNow.AddSeconds(300)
            };

            showing.Reservations ??= [];
            showing.Reservations.Add(reservation);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return reservation.Id;
        }
    }
}

public class AddReservationEndpoint : IEndpoint
{
    public record AddReservationRequest(Guid SeatId);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "showings/{showingId:guid}/reservations",
            async (Guid showingId, AddReservationRequest request, ISender sender) =>
            {
                var result = await sender.Send(request.ToCommand(showingId));
                if (result.IsFailure) return result.Error.ToResult();

                return Results.CreatedAtRoute(
                    nameof(GetReservationEndpoint),
                    new { ShowingId = showingId, reservationId = result.Value },
                    new { Id = result.Value }
                );
            }
        )
        .WithName(nameof(AddReservationEndpoint))
        .WithTags("Showings");
    }
}

public static class AddReservationMapper
{
    public static AddReservation.Command ToCommand(this AddReservationEndpoint.AddReservationRequest request, Guid showingId)
    {
        return new()
        {
            ShowingId = showingId,
            SeatId = request.SeatId
        };
    }
}