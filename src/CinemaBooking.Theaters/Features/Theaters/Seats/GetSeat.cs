namespace CinemaBooking.Theaters.Features.Theaters.Seats;

public static class GetSeat
{
    public class Query : IRequest<Result<Seat>>
    {
        public required Guid TheaterId { get; set; }
        public required Guid SeatId { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(c => c.SeatId).NotEmpty();
            RuleFor(c => c.TheaterId).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Seat>>
    {
        private readonly IValidator<Query> _validator;
        private readonly IMediator _mediator;

        public Handler(IValidator<Query> validator, IMediator mediator)
        {
            _validator = validator;
            _mediator = mediator;
        }

        public async Task<Result<Seat>> Handle(Query request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return SeatError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var theaterResult = await _mediator
                .Send(new GetTheater.Query { TheaterId = request.TheaterId }, cancellationToken);
            if (theaterResult.IsFailure) return theaterResult.Error;
            var theater = theaterResult.Value;

            if (theater.Seats is null) return SeatError.NotFound;

            var seat = theater.Seats
                .SingleOrDefault(s => s.Id == request.SeatId);

            if (seat is null) return SeatError.NotFound;

            return seat;
        }
    }
}

public class GetSeatEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("theaters/{theaterId:guid}/seats/{seatId:guid}", async (Guid theaterId, Guid seatId, ISender sender) =>
        {
            var result = await sender.Send(new GetSeat.Query { TheaterId = theaterId, SeatId = seatId });
            if (result.IsFailure) return result.Error.ToResult();

            return Results.Ok(result.Value.ToResponse());
        })
        .WithName(nameof(GetSeatEndpoint))
        .WithTags("Theaters");
    }
}