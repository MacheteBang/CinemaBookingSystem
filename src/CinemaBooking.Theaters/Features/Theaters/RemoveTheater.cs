namespace CinemaBooking.Theaters.Features.Theaters;

public static class RemoveTheater
{
    public class Command : IRequest<Result>
    {
        public required Guid Id { get; set; }
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
            IQueryable<Theater> getTheaters() => _dbContext.Theaters.Where(m => m.Id == request.Id);

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

    public record Request(Guid Id);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("theaters/{id:guid}", async ([AsParameters] Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand());
            if (result.IsFailure) return result.Error.ToResult();

            return Results.NoContent();
        })
        .WithName(nameof(RemoveTheaterEndpoint))
        .WithTags("Theaters");
    }
}

public static class RemoveTheaterMapper
{
    public static RemoveTheater.Command ToCommand(this RemoveTheaterEndpoint.Request request)
    {
        return new()
        {
            Id = request.Id
        };
    }
}