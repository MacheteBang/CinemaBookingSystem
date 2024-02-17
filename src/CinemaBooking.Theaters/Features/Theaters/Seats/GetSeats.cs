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
        private readonly TheatersDbContext _dbContext;

        public Handler(IValidator<Query> validator, TheatersDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async Task<Result<List<Seat>>> Handle(Query request, CancellationToken cancellationToken)
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

            return theater.Seats;
        }
    }
}

public class GetSeatsEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("theaters/{theaterId:guid}/seats", async (Guid theaterId, ISender sender) =>
        {
            GetSeats.Query query = new()
            {
                TheaterId = theaterId
            };

            var result = await sender.Send(query);

            return result.IsSuccess ? Results.Ok(result.Value.Select(s => s.ToResponse()))
                : result.Error.Code switch
                {
                    SeatError.Codes.NotFound => Results.NotFound(result.Error.Messages),
                    SeatError.Codes.TheaterNotFound => Results.NotFound(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(GetSeatsEndpoint))
        .WithTags("Theaters");
    }
}