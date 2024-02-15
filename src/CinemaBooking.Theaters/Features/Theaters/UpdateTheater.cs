namespace CinemaBooking.Theaters.Features.Theaters;

public static class UpdateTheater
{
    public class Command : IRequest<Result>
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string SeatingArrangement { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Id).NotEmpty();
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

            Theater theater = new()
            {
                Id = request.Id,
                Name = request.Name,
                SeatingArrangement = request.SeatingArrangement
            };

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
        app.MapPut("theaters/{id:guid}", async (Guid id, Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand(id));

            return result.IsSuccess ? Results.Accepted()
                : result.Error.Code switch
                {
                    TheaterError.Codes.Invalid => Results.BadRequest(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(UpdateTheaterEndpoint))
        .WithTags("Theaters");
    }
}

public static class UpdateTheaterMapper
{
    public static UpdateTheater.Command ToCommand(this UpdateTheaterEndpoint.Request request, Guid id)
    {
        return new()
        {
            Id = id,
            Name = request.Name,
            SeatingArrangement = request.SeatingArrangement
        };
    }
}