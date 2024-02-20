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
        private readonly IMediator _mediator;

        public Handler(TheatersDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<Result<List<Reservation>>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Validate that the Showing exists. If not, an error should be returned.
            var showingResult = await _mediator
                .Send(new GetShowing.Query { ShowingId = request.ShowingId }, cancellationToken);
            if (showingResult.IsFailure) return showingResult.Error;
            var showing = showingResult.Value;

            return (showing.Reservations ?? [])
                .Where(r => r.IsActive())
                .ToList();
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
                var result = await sender.Send(new GetReservations.Query { ShowingId = showingId });
                if (result.IsFailure) return result.Error.ToResult();

                return Results.Ok(result.Value.Select(r => r.ToResponse()));
            }
        )
        .WithName(nameof(GetReservationsEndpoint))
        .WithTags("Showings");
    }
}