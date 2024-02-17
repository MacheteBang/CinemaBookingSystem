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
        private readonly TheatersDbContext _dbContext;

        public Handler(IValidator<Query> validator, TheatersDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async Task<Result<Seat>> Handle(Query request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return SeatError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var theater = await _dbContext.Theaters
                .SingleOrDefaultAsync(sh => sh.Id == request.TheaterId);

            if (theater is null) return SeatError.TheaterNotFound;
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

            return result.IsSuccess ? Results.Ok(result.Value.ToResponse())
                : result.Error.Code switch
                {
                    SeatError.Codes.NotFound => Results.NotFound(result.Error.Messages),
                    SeatError.Codes.TheaterNotFound => Results.NotFound(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(GetSeatEndpoint))
        .WithTags("Theaters");
    }
}