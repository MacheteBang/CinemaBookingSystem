namespace CinemaBooking.Theaters.Database;

public class TheatersDbContext : DbContext
{
    public TheatersDbContext(DbContextOptions<TheatersDbContext> options) : base(options) { }

    public DbSet<Theater> Theaters { get; set; }
    public DbSet<Showing> Showings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Theater>()
            .OwnsMany(s => s.Seats);

        modelBuilder.Entity<Showing>()
            .OwnsMany(s => s.Reservations);
    }
}
