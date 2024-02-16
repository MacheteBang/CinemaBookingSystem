namespace CinemaBooking.Theaters.Features.Showings.Seats;

public static class GetSeats
{
    public class Query : IRequest<Result<List<Seat>>>
    {
        public required Guid ShowingId { get; set; }
        public bool? IsAvailable { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(c => c.ShowingId).NotEmpty();
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

            var showing = await _dbContext.Showings
                .SingleOrDefaultAsync(sh => sh.Id == request.ShowingId);

            if (showing is null) return SeatError.ShowingNotFound;
            if (showing.Seats is null) return SeatError.NotFound;

            var filteredSeats = showing.Seats
                .Where(s => request.IsAvailable is null || request.IsAvailable == s.IsAvailable);

            return filteredSeats.ToList();
        }
    }
}

public class GetSeatsEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("showings/{showingId:guid}/seats", async (Guid showingId, bool? isAvailable, ISender sender) =>
        {
            GetSeats.Query query = new()
            {
                ShowingId = showingId,
                IsAvailable = isAvailable
            };

            var result = await sender.Send(query);

            return result.IsSuccess ? Results.Ok(result.Value.Select(s => s.ToResponse()))
                : result.Error.Code switch
                {
                    SeatError.Codes.NotFound => Results.NotFound(result.Error.Messages),
                    SeatError.Codes.ShowingNotFound => Results.NotFound(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(GetSeatsEndpoint))
        .WithTags("Showings");
    }
}