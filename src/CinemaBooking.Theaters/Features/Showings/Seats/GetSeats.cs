namespace CinemaBooking.Theaters.Features.Showings.Seats;

public static class GetSeats
{
    public class Query : IRequest<Result<List<Seat>>>
    {
        public required Guid ShowingId { get; set; }
        public Seat.OccupancyState? Occupancy { get; set; }
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
                return SeatErrors.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var showing = await _dbContext.Showings
                .SingleOrDefaultAsync(sh => sh.Id == request.ShowingId);

            if (showing is null) return SeatErrors.ShowingNotFound;
            if (showing.Seats is null) return SeatErrors.NotFound;

            var filteredSeats = showing.Seats
                .Where(s => request.Occupancy is null || s.Occupancy == request.Occupancy);

            return filteredSeats.ToList();
        }
    }
}

public class GetSeatsEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("showings/{showingId:guid}/seats", async (Guid showingId, Seat.OccupancyState? occupancy, ISender sender) =>
        {
            GetSeats.Query query = new()
            {
                ShowingId = showingId,
                Occupancy = occupancy
            };

            var result = await sender.Send(query);

            return result.IsSuccess ? Results.Ok(result.Value.Select(s => s.ToResponse()))
                : result.Error.Code switch
                {
                    SeatErrors.Codes.NotFound => Results.NotFound(result.Error.Messages),
                    SeatErrors.Codes.ShowingNotFound => Results.NotFound(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(GetSeatsEndpoint))
        .WithTags("Showings");
    }
}