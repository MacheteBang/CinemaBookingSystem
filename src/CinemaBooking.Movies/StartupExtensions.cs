using System.Reflection;
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

        Assembly moviesAssembly = typeof(StartupExensions).Assembly;

        services.AddDbContext<MoviesDbContext>(dbOptions =>
        {
            if (options.DbProvider == MoviesDbProvider.InMemory) dbOptions.UseInMemoryDatabase("Movies");
            if (options.DbProvider == MoviesDbProvider.Sqlite) dbOptions.UseSqlite(options.DbConnectionString);
        });

        if (options.DbProvider == MoviesDbProvider.Sqlite)
        {
            using var serviceScope = services.BuildServiceProvider().CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<MoviesDbContext>();
            dbContext.Database.EnsureCreated();
        }

        services.AddMediatR(mediatROptions => mediatROptions.RegisterServicesFromAssembly(moviesAssembly));
        services.AddValidatorsFromAssembly(moviesAssembly);

        services.AddCarter();

        return services;
    }

    public static WebApplication UseMovies(this WebApplication app)
    {
        app.MapCarter();

        return app;
    }
}