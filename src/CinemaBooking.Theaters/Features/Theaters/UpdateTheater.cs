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
        private readonly IMediator _mediator;

        public Handler(IValidator<Command> validator, TheatersDbContext dbContext, IMediator mediator)
        {
            _validator = validator;
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return TheaterError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var theaterResult = await _mediator
                .Send(new GetTheater.Query { TheaterId = request.TheaterId }, cancellationToken);
            if (theaterResult.IsFailure) return theaterResult.Error;
            var theater = theaterResult.Value;

            theater.Name = request.Name;
            // Because seats are tied to reservations, changing them will require
            // updating those reservations.
            // TODO: Handle SeatingArrangement changes inconsideration of Reservations

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