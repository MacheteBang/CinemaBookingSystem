namespace CinemaBooking.Movies.Features.Genres;

public static class GetGenres
{
    public class Query : IRequest<Result<ICollection<string>>> { }

    internal sealed class Handler : IRequestHandler<Query, Result<ICollection<string>>>
    {
        public async Task<Result<ICollection<string>>> Handle(Query request, CancellationToken cancellationToken)
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

            return result.IsSuccess ? Results.Ok(result.Value)
                : result.Error.Code switch
                {
                    _ => Results.BadRequest()
                };

        })
        .WithName(nameof(GetGenresEndpoint))
        .WithTags("Genres");
    }
}