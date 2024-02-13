namespace CinemaBooking.Theaters.Features.Showings;

public static class GetShowing
{
    public class Query : IRequest<Result<Showing>>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Showing>>
    {
        private readonly TheatersDbContext _dbContext;

        public Handler(TheatersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Showing>> Handle(Query request, CancellationToken cancellationToken)
        {
            var showing = await _dbContext.Showings
                .Where(t => t.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (showing is null) return ShowingErrors.NotFound;

            return showing;
        }
    }
}

public class GetShowingEndpoint : IEndpoint
{
    public record Request(Guid Id);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("showings/{id:guid}", async ([AsParameters] Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToQuery());

            return result.IsSuccess ? Results.Ok(result.Value.ToResponse())
                : result.Error.Code switch
                {
                    ShowingErrors.Codes.NotFound => Results.NotFound(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(GetShowingEndpoint))
        .WithTags("Showings");
    }
}

public static class GetShowingMapper
{
    public static GetShowing.Query ToQuery(this GetShowingEndpoint.Request request)
    {
        return new()
        {
            Id = request.Id
        };
    }
}