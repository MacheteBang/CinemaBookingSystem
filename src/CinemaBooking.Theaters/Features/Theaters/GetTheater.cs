namespace CinemaBooking.Theaters.Features.Theaters;

public static class GetTheater
{
    public class Query : IRequest<Result<Theater>>
    {
        public required Guid TheaterId { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(c => c.TheaterId).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Theater>>
    {
        private readonly IValidator<Query> _validator;
        private readonly TheatersDbContext _dbContext;

        public Handler(IValidator<Query> validator, TheatersDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async Task<Result<Theater>> Handle(Query request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return TheaterError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var theater = await _dbContext.Theaters
                .Where(t => t.Id == request.TheaterId)
                .SingleOrDefaultAsync(cancellationToken);

            if (theater is null) return TheaterError.NotFound;

            return theater;
        }
    }
}

public class GetTheaterEndpoint : IEndpoint
{
    public record Request(Guid Id);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("theaters/{theaterId:guid}", async (Guid theaterId, ISender sender) =>
        {
            var result = await sender.Send(new GetTheater.Query() { TheaterId = theaterId });
            if (result.IsFailure) return result.Error.ToResult();

            return Results.Ok(result.Value.ToResponse(false));
        })
        .WithName(nameof(GetTheaterEndpoint))
        .WithTags("Theaters");
    }
}