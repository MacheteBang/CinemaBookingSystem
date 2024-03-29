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
                Seats = SeatingArrangement
                    .GetSeatingArrangement(request.SeatingArrangement)
                    .CreateSeats()
            };

            await _dbContext.Theaters.AddAsync(theater, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return theater.Id;
        }
    }
}

public class AddTheaterEndpoint : IEndpoint
{
    public record AddTheaterRequest(string Name, string SeatingArrangement);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("theaters", async (AddTheaterRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand());
            if (result.IsFailure) return result.Error.ToResult();

            return Results.CreatedAtRoute(
                nameof(GetTheaterEndpoint),
                new { TheaterId = result.Value },
                new { Id = result.Value }
            );
        })
        .WithName(nameof(AddTheaterEndpoint))
        .WithTags("Theaters");
    }
}

public static class AddTheaterMapper
{
    public static AddTheater.Command ToCommand(this AddTheaterEndpoint.AddTheaterRequest request)
    {
        return new()
        {
            Name = request.Name,
            SeatingArrangement = request.SeatingArrangement
        };
    }
}