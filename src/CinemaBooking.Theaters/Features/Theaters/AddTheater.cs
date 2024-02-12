
using CinemaBooking.Theaters.Featers.Theaters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CinemaBooking.Theaters.Features.Theaters;

public static class AddTheater
{
    public class Command : IRequest<Result<Guid>>
    {
        public string Name { get; set; } = string.Empty;
        public string SeatingArrangement { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.SeatingArrangement).NotNull();
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
                return TheaterErrors.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            Theater theater = new()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Seats = SeatingArrangement.GetSeatingArrangement(request.SeatingArrangement)
            };

            _dbContext.Theaters.Add(theater);
            _dbContext.SaveChanges();

            return await Task.FromResult(theater.Id);
        }
    }
}

public class AddTheaterEndpoint : IEndpoint
{
    public record Request(string Name, string SeatingArrangement);

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("theaters", async (Request request, ISender sender) =>
        {
            var result = await sender.Send(request.ToCommand());

            return result.IsSuccess ? Results.CreatedAtRoute(nameof(GetTheaterEndpoint), new { Id = result.Value }, new { Id = result.Value })
                : result.Error.Code switch
                {
                    TheaterErrors.Codes.Invalid => Results.BadRequest(result.Error.Messages),
                    _ => Results.BadRequest()
                };
        })
        .WithName(nameof(AddTheaterEndpoint))
        ;
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