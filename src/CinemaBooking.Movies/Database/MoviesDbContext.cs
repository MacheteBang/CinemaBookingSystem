namespace CinemaBooking.Movies.Database;

public class MoviesDbContext : DbContext
{
    public MoviesDbContext(DbContextOptions<MoviesDbContext> options) : base(options) { }

    public DbSet<Movie> Movies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>()
            .Property(m => m.Genres)
            .HasConversion(
                o => string.Join(',', o),
                o => o.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(v => Enum.Parse<Genre>(v)).ToList()
            );
    }
}
