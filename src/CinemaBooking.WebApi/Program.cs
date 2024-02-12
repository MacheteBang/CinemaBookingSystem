var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration)
);

// builder.Services.Configure<JsonOptions>(options =>
// {
//     options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
// });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMovies(moviesOptions =>
{
    moviesOptions.DbProvider = DbProvider.Sqlite;
    moviesOptions.DbConnectionString = "Datasource=movies.db";
    moviesOptions.UseEndpoints = true;
});

builder.Services.AddTheaters(theatersOptions =>
{
    theatersOptions.DbProvider = DbProvider.Sqlite;
    theatersOptions.DbConnectionString = "Datasource=theaters.db";
    theatersOptions.UseEndpoints = true;
});

var app = builder.Build();

app.UseExceptionHandler(exHandler => exHandler.Run(async context =>
    await Results.Problem().ExecuteAsync(context)));
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMovies();
app.UseTheaters();

app.Run();