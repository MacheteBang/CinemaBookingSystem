using CinemaBooking.Theaters.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CinemaBooking.Movies;

public static class StartupExensions
{
    public static IServiceCollection AddTheaters(this IServiceCollection services,
        Action<TheatersOptions> optionsAction)
    {
        TheatersOptions options = new();
        optionsAction(options);

        services.AddSingleton(typeof(IOptions<TheatersOptions>), Microsoft.Extensions.Options.Options.Create(options));

        return services;
    }

    public static WebApplication UseTheaters(this WebApplication app)
    {
        return app;
    }
}