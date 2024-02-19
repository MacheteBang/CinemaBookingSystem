namespace CinemaBooking.Theaters.Features.Theaters.Seats;

public static class GetSeats
{
    public class Query : IRequest<Result<List<Seat>>>
    {
        public required Guid TheaterId { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(c => c.TheaterId).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<List<Seat>>>
    {
        private readonly IValidator<Query> _validator;
        private readonly IMediator _mediator;

        public Handler(IValidator<Query> validator, IMediator mediator)
        {
            _validator = validator;
            _mediator = mediator;
        }

        public async Task<Result<List<Seat>>> Handle(Query request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return SeatError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var theaterResult = await _mediator.Send(new GetTheater.Query { TheaterId = request.TheaterId }, cancellationToken);
            if (theaterResult.IsFailure) return theaterResult.Error;
            var theater = theaterResult.Value;

            return theater.Seats ?? [];
        }
    }
}

public class GetSeatsEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("theaters/{theaterId:guid}/seats", async (Guid theaterId, ISender sender) =>
        {
            var result = await sender.Send(new GetSeats.Query { TheaterId = theaterId });
            if (result.IsFailure) return result.Error.ToResult();

            return Results.Ok(result.Value.Select(s => s.ToResponse()));
        })
        .WithName(nameof(GetSeatsEndpoint))
        .WithTags("Theaters");
    }
}