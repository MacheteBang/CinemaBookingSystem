using System.Reflection;
using CinemaBooking.Theaters.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CinemaBooking.Theaters;

public static class StartupExensions
{
    public static IServiceCollection AddTheaters(this IServiceCollection services,
        Action<TheatersOptions> optionsAction)
    {
        TheatersOptions options = new();
        optionsAction(options);

        services.AddSingleton(typeof(IOptions<TheatersOptions>), Microsoft.Extensions.Options.Options.Create(options));

        Assembly thisAssembly = typeof(StartupExensions).Assembly;

        services.AddDbContext<TheatersDbContext>(dbOptions =>
        {
            if (options.DbProvider == DbProvider.InMemory) dbOptions.UseInMemoryDatabase("Theaters");
            if (options.DbProvider == DbProvider.Sqlite) dbOptions.UseSqlite(options.DbConnectionString);
        });

        if (options.DbProvider == DbProvider.Sqlite)
        {
            using var serviceScope = services.BuildServiceProvider().CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<TheatersDbContext>();
            dbContext.Database.EnsureCreated();
        }

        services.AddMediatR(mediatROptions => mediatROptions.RegisterServicesFromAssembly(thisAssembly));
        services.AddValidatorsFromAssembly(thisAssembly);

        return services;
    }

    public static WebApplication UseTheaters(this WebApplication app)
    {
        var theatersOptions = app.Services.GetService<IOptions<TheatersOptions>>()
            ?? throw new Exception("Theaters options missing");

        if (theatersOptions.Value.UseEndpoints)
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