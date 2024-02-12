namespace CinemaBooking.Movies.Features.Genres;

public static class GetGenres
{
    public class Query : IRequest<IResult> { }

    internal sealed class Handler : IRequestHandler<Query, IResult>
    {
        public async Task<IResult> Handle(Query request, CancellationToken cancellationToken)
        {
            ICollection<string> genres = Enum.GetValues(typeof(Genre))
                .Cast<Genre>()
                .Select(e => e.ToString())
                .ToList();

            return await Task.FromResult(Results.Ok(genres));
        }
    }
}

public class GetGenresEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("genres", async (ISender sender) =>
        {
            return await sender.Send(new GetGenres.Query());
        })
        .WithName(nameof(GetGenresEndpoint))
        .WithTags("Genres");
    }
}