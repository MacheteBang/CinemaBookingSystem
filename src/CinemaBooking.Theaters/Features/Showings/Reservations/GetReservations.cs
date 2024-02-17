namespace CinemaBooking.Theaters.Features.Showings.Reservations;

public static class GetReservations
{
    public class Query : IRequest<Result<List<Reservation>>>
    {
        public required Guid ShowingId { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<List<Reservation>>>
    {
        private readonly TheatersDbContext _dbContext;

        public Handler(TheatersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<Reservation>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var reservations = await _dbContext.Reservations
                .Where(r => r.ShowingId == request.ShowingId)
                .ToListAsync(cancellationToken);

            return reservations ?? [];
        }
    }
}

public class GetReservationsEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "showings/{showingId:guid}/reservations",
            async (Guid showingId, ISender sender) =>
            {
                var result = await sender.Send(new GetReservations.Query() { ShowingId = showingId });

                return result.IsSuccess ? Results.Ok(result.Value.Select(r => r.ToResponse()))
                    : result.Error.Code switch
                    {
                        ReservationError.Codes.NotFound => Results.NotFound(result.Error.Messages),
                        _ => Results.BadRequest()
                    };
            }
        )
        .WithName(nameof(GetReservationsEndpoint))
        .WithTags("Showings");
    }
}