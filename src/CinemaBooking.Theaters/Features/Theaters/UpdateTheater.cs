namespace CinemaBooking.Theaters.Features.Theaters;

public static class UpdateTheater
{
    public class Command : IRequest<Result>
    {
        public required Guid TheaterId { get; set; }
        public required string Name { get; set; }
        public string? SeatingArrangement { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.TheaterId).NotEmpty();
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.SeatingArrangement).NotNull();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result>
    {
        private readonly IValidator<Command> _validator;
        private readonly TheatersDbContext _dbContext;

        public Handler(IValidator<Command> validator, TheatersDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return TheaterError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var theater = await _dbContext.Theaters
                .SingleOrDefaultAsync(t => t.Id == request.TheaterId, cancellationToken);

            if (theater is null) return TheaterError.NotFound;

            theater.Name = request.Name;

            if (request.SeatingArrangement is not null)
            {
                theater.Seats = SeatingArrangement
                    .GetSeatingArrangement(request.SeatingArrangement)
                    .CreateSeats();
            }

            _dbContext.Theaters.Update(theater);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

public class UpdateTheaterEndpoint : IEndpoint
{
    public record Request(
        string Name,
        string SeatingArrangement
    );


    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("theaters/{theaterId:guid}", async (Guid theaterId, Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand(theaterId));
            if (result.IsFailure) return result.Error.ToResult();

            return Results.Accepted();
        })
        .WithName(nameof(UpdateTheaterEndpoint))
        .WithTags("Theaters");
    }
}

public static class UpdateTheaterMapper
{
    public static UpdateTheater.Command ToCommand(this UpdateTheaterEndpoint.Request request, Guid theaterId)
    {
        return new()
        {
            TheaterId = theaterId,
            Name = request.Name,
            SeatingArrangement = request.SeatingArrangement
        };
    }
}