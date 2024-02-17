namespace CinemaBooking.Theaters.Features.Theaters;

public static class RemoveTheater
{
    public class Command : IRequest<Result>
    {
        public required Guid TheaterId { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        public readonly TheatersDbContext _dbContext;

        public Handler(TheatersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            IQueryable<Theater> getTheaters() => _dbContext.Theaters.Where(m => m.Id == request.TheaterId);

            if (_dbContext.Database.IsRelational())
            {
                await getTheaters().ExecuteDeleteAsync(cancellationToken);
            }
            else
            {
                _dbContext.RemoveRange(getTheaters());
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }
    }
}

public class RemoveTheaterEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("theaters/{theaterId:guid}", async (Guid theaterId, ISender sender) =>
        {
            var result = await sender.Send(new RemoveTheater.Command { TheaterId = theaterId });
            if (result.IsFailure) return result.Error.ToResult();

            return Results.NoContent();
        })
        .WithName(nameof(RemoveTheaterEndpoint))
        .WithTags("Theaters");
    }
}