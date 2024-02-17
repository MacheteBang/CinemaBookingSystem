namespace CinemaBooking.Theaters.Features.Theaters;

public static class GetTheater
{
    public class Query : IRequest<Result<Theater>>
    {
        public required Guid Id { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(c => c.Id).NotEmpty();
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
                .Where(t => t.Id == request.Id)
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
        app.MapGet("theaters/{id:guid}", async ([AsParameters] Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToQuery());

            return result.IsSuccess ? Results.Ok(result.Value.ToResponse(false))
                : result.Error.Code switch
                {
                    TheaterError.Codes.NotFound => Results.NotFound(result.Error.Messages),
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