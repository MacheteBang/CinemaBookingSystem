using CinemaBooking.Movies.Options;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaBooking.Movies;

public static class StartupExensions
{
    public static IServiceCollection AddMovies(this IServiceCollection services,
        Action<MoviesOptions> optionsAction)
    {
        MoviesOptions options = new();
        optionsAction(options);

        services.AddDbContext<MoviesDbContext>(dbOptions =>
        {
            if (options.DbProvider == MoviesDbProvider.InMemory) dbOptions.UseInMemoryDatabase("Movies");
        });

        return services;
    }
}