using System.Reflection;
using CinemaBooking.Movies.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CinemaBooking.Movies;

public static class StartupExensions
{
    public static IServiceCollection AddMovies(this IServiceCollection services,
        Action<MoviesOptions> optionsAction)
    {
        MoviesOptions options = new();
        optionsAction(options);

        services.AddSingleton(typeof(IOptions<MoviesOptions>), Microsoft.Extensions.Options.Options.Create(options));

        Assembly thisAssembly = typeof(StartupExensions).Assembly;

        services.AddDbContext<MoviesDbContext>(dbOptions =>
        {
            if (options.DbProvider == DbProvider.InMemory) dbOptions.UseInMemoryDatabase("Movies");
            if (options.DbProvider == DbProvider.Sqlite) dbOptions.UseSqlite(options.DbConnectionString);
        });

        if (options.DbProvider == DbProvider.Sqlite)
        {
            using var serviceScope = services.BuildServiceProvider().CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<MoviesDbContext>();
            dbContext.Database.EnsureCreated();
        }

        services.AddMediatR(mediatROptions => mediatROptions.RegisterServicesFromAssembly(thisAssembly));
        services.AddValidatorsFromAssembly(thisAssembly);

        return services;
    }

    public static WebApplication UseMovies(this WebApplication app)
    {
        var moviesOptions = app.Services.GetService<IOptions<MoviesOptions>>()
            ?? throw new Exception("Movies options missing");

        if (moviesOptions.Value.UseEndpoints)
        {
            app.MapEndpoints();
        }

        return app;
    }

    private static void MapEndpoints(this WebApplication app)
    {
        Assembly thisAssembly = typeof(StartupExensions).Assembly;

        var endpoints = thisAssembly.GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && t.IsClass && !t.IsInterface && !t.IsAbstract);

        foreach (var endpoint in endpoints)
        {
            var instance = Activator.CreateInstance(endpoint) as IEndpoint;
            instance?.AddRoutes(app);
        }
    }
}