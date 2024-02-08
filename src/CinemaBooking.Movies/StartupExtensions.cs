using Microsoft.Extensions.DependencyInjection;

namespace CinemaBooking.Movies;

public static class StartupExensions
{
    public static IServiceCollection AddMovies(this IServiceCollection services)
    {
        return services;
    }
}