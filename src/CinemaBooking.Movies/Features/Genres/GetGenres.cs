namespace CinemaBooking.Movies.Features.Genres;

public static class GetGenres
{
    public class Query : IRequest<Result<List<string>>> { }

    internal sealed class Handler : IRequestHandler<Query, Result<List<string>>>
    {
        public async Task<Result<List<string>>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<string> genres = Enum.GetValues(typeof(Genre))
                .Cast<Genre>()
                .Select(e => e.ToString())
                .ToList();

            return await Task.FromResult(genres);
        }
    }
}

public class GetGenresEndpoint : IEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("genres", async (ISender sender) =>
        {
            var result = await sender.Send(new GetGenres.Query());
            if (result.IsFailure) return result.Error.ToResult();

            return Results.Ok(result.Value);

        })
        .WithName(nameof(GetGenresEndpoint))
        .WithTags("Genres");
    }
}