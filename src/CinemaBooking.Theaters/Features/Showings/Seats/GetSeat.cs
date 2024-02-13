namespace CinemaBooking.Theaters.Features.Showings.Seats;

public static class GetSeat
{
    public class Query : IRequest<Result<Seat>>
    {
        public required Guid ShowingId { get; set; }
        public required Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Seat>>
    {
        private readonly TheatersDbContext _dbContext;

        public Handler(TheatersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Seat>> Handle(Query request, CancellationToken cancellationToken)
        {
            var showing = await _dbContext.Showings
                .SingleOrDefaultAsync(sh => sh.Id == request.ShowingId);

            if (showing is null) return SeatErrors.ShowingNotFound;
            if (showing.Seats is null) return SeatErrors.NotFound;

            var seat = showing.Seats
                .SingleOrDefault(s => s.Id == request.Id);

            if (seat is null) return SeatErrors.NotFound;

            return seat;
        }
    }
}

public class GetSeatEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("showings/{showingId:guid}/seats/{id:guid}", async (Guid showingId, Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetSeat.Query { ShowingId = showingId, Id = id });

            return result.IsSuccess ? Results.Ok(result.Value.ToResponse())
                : result.Error.Code switch
                {
                    SeatErrors.Codes.NotFound => Results.NotFound(result.Error.Messages),
                    SeatErrors.Codes.ShowingNotFound => Results.NotFound(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(GetSeatEndpoint))
        .WithTags("Showings");
    }
}