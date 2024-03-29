using Microsoft.EntityFrameworkCore.Update.Internal;

namespace CinemaBooking.Theaters.Features.Showings;

public static class GetShowing
{
    public class Query : IRequest<Result<Showing>>
    {
        public Guid ShowingId { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(c => c.ShowingId).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<Showing>>
    {
        private readonly IValidator<Query> _validator;
        private readonly TheatersDbContext _dbContext;

        public Handler(IValidator<Query> validator, TheatersDbContext dbContext)
        {
            _validator = validator;
            _dbContext = dbContext;
        }

        public async Task<Result<Showing>> Handle(Query request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ShowingError.Validation(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var showing = await _dbContext.Showings
                .SingleOrDefaultAsync(t => t.Id == request.ShowingId, cancellationToken);

            if (showing is null) return ShowingError.NotFound;

            return showing;
        }
    }
}

public class GetShowingEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("showings/{showingId:guid}", async (Guid showingId, ISender sender) =>
        {
            var result = await sender.Send(new GetShowing.Query { ShowingId = showingId });
            if (result.IsFailure) return result.Error.ToResult();

            return Results.Ok(result.Value.ToResponse());
        })
        .WithName(nameof(GetShowingEndpoint))
        .WithTags("Showings");
    }
}