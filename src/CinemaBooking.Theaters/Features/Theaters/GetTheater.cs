namespace CinemaBooking.Theaters.Features.Theaters;

public static class GetTheater
{
    public class Query : IRequest<Result<Theater>>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Theater>>
    {
        private readonly TheatersDbContext _dbContext;

        public Handler(TheatersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Theater>> Handle(Query request, CancellationToken cancellationToken)
        {
            var theater = await _dbContext.Theaters
                .Where(t => t.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (theater is null) return TheaterErrors.NotFound;

            return await Task.FromResult(theater);
        }
    }
}

public class GetTheaterEndpoint : IEndpoint
{
    public record Request(Guid Id);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("theaters/{id:guid}", async ([AsParameters] Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToQuery());

            return result.IsSuccess ? Results.Ok(result.Value.ToResponse())
                : result.Error.Code switch
                {
                    TheaterErrors.Codes.NotFound => Results.NotFound(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(GetTheaterEndpoint))
        .WithTags("Theaters");
    }
}

public static class GetTheaterMapper
{
    public static GetTheater.Query ToQuery(this GetTheaterEndpoint.Request request)
    {
        return new()
        {
            Id = request.Id
        };
    }
}