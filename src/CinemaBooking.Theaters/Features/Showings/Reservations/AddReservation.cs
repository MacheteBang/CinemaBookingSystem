
using System.ComponentModel;
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
        public readonly IMediator _mediatr;

        public Handler(IValidator<Command> validator, TheatersDbContext dbContext, IMediator mediatr)
        {
            _dbContext = dbContext;
            _validator = validator;
            _mediatr = mediatr;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ReservationError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var showingQuery = new GetShowing.Query() { ShowingId = request.ShowingId };
            var showingResult = await _mediatr.Send(showingQuery, cancellationToken);
            if (showingResult.IsFailure) return showingResult.Error;
            var showing = showingResult.Value;

            var seatQuery = new GetSeat.Query() { TheaterId = showing.TheaterId, SeatId = request.SeatId };
            var seatResult = await _mediatr.Send(seatQuery, cancellationToken);
            if (seatResult.IsFailure) return seatResult.Error;
            var seat = seatResult.Value;

            // TODO: Add logic to confirm one seat reservation per showing.
            // TODO: Add logic to mark a seat as pending.

            Reservation reservation = new()
            {
                Id = Guid.NewGuid(),
                ShowingId = request.ShowingId,
                SeatId = request.SeatId
            };

            _dbContext.Reservations.Add(reservation);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ShowingError.NotFound;
        }
    }
}

public class AddReservationEndpoint : IEndpoint
{
    public record Request(Guid SeatId);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "showings/{showingId:guid}/reservations",
            async (Guid showingId, Request request, ISender sender) =>
            {
                var result = await sender.Send(request.ToCommand(showingId));
                if (result.IsFailure) return result.Error.ToResult();

                return Results.Ok(); // TODO: Change to CreatedAtRoute
            }
        )
        .WithName(nameof(AddReservationEndpoint))
        .WithTags("Showings");
    }
}

public static class AddReservationMapper
{
    public static AddReservation.Command ToCommand(this AddReservationEndpoint.Request request, Guid showingId)
    {
        return new()
        {
            ShowingId = showingId,
            SeatId = request.SeatId
        };
    }
}