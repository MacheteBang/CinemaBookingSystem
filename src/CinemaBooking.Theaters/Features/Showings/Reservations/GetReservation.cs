namespace CinemaBooking.Theaters.Features.Theaters.Reservations;

public static class GetReservation
{
    public class Query : IRequest<Result<Reservation>>
    {
        public required Guid ShowingId { get; set; }
        public required Guid ReservationId { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(c => c.ShowingId).NotEmpty();
            RuleFor(c => c.ReservationId).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Reservation>>
    {
        private readonly IValidator<Query> _validator;
        private readonly IMediator _mediator;

        public Handler(IValidator<Query> validator, IMediator mediator)
        {
            _validator = validator;
            _mediator = mediator;
        }

        public async Task<Result<Reservation>> Handle(Query request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ReservationError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var showingResult = await _mediator
                .Send(new GetShowing.Query { ShowingId = request.ShowingId }, cancellationToken);
            if (showingResult.IsFailure) return showingResult.Error;
            var showing = showingResult.Value;

            if (showing.Reservations is null) return ReservationError.NotFound;

            var reservation = showing.Reservations
                .SingleOrDefault(s => s.Id == request.ReservationId);

            if (reservation is null) return ReservationError.NotFound;

            return reservation;
        }
    }
}

public class GetReservationEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("showings/{showingId:guid}/reservations/{reservationId:guid}", async (Guid showingId, Guid reservationId, ISender sender) =>
        {
            var result = await sender.Send(new GetReservation.Query { ShowingId = showingId, ReservationId = reservationId });
            if (result.IsFailure) return result.Error.ToResult();

            return Results.Ok(result.Value.ToResponse());
        })
        .WithName(nameof(GetReservationEndpoint))
        .WithTags("Showings");
    }
}