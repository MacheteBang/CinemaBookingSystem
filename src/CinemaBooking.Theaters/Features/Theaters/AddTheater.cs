namespace CinemaBooking.Theaters.Features.Theaters;

public static class AddTheater
{
    public class Command : IRequest<Result<Guid>>
    {
        public required string Name { get; set; }
        public required string SeatingArrangement { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.SeatingArrangement).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<Guid>>
    {
        public readonly IValidator<Command> _validator;
        public readonly TheatersDbContext _dbContext;

        public Handler(IValidator<Command> validator, TheatersDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return TheaterError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            Theater theater = new()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                SeatingArrangement = SeatingArrangement.GetSeatingArrangement(request.SeatingArrangement)
            };

            await _dbContext.Theaters.AddAsync(theater, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return theater.Id;
        }
    }
}

public class AddTheaterEndpoint : IEndpoint
{
    public record Request(string Name, string SeatingArrangement);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("theaters", async ([AsParameters] Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand());

            return result.IsSuccess ? Results.CreatedAtRoute(nameof(GetTheaterEndpoint), new { Id = result.Value }, new { Id = result.Value })
                : result.Error.Code switch
                {
                    TheaterError.Codes.Invalid => Results.BadRequest(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(AddTheaterEndpoint))
        .WithTags("Theaters");
    }
}

public static class AddTheaterMapper
{
    public static AddTheater.Command ToCommand(this AddTheaterEndpoint.Request request)
    {
        return new()
        {
            Name = request.Name,
            SeatingArrangement = request.SeatingArrangement
        };
    }
}