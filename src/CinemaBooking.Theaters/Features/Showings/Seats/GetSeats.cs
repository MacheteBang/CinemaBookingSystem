

namespace CinemaBooking.Theaters.Features.Showings.Seats;

public static class GetSeats
{
    public class Query : IRequest<Result<List<Seat>>>
    {
        public required Guid ShowingId { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<List<Seat>>>
    {
        private readonly TheatersDbContext _dbContext;

        public Handler(TheatersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<Seat>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var showing = await _dbContext.Showings
                .SingleOrDefaultAsync(sh => sh.Id == request.ShowingId);

            if (showing is null) return SeatErrors.ShowingNotFound;
            if (showing.Seats is null) return SeatErrors.NotFound;

            return showing.Seats;
        }
    }
}

public class GetSeatsEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("showings/{showingId:Guid}/seats", async (Guid showingId, ISender sender) =>
        {
            var result = await sender.Send(new GetSeats.Query { ShowingId = showingId });

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