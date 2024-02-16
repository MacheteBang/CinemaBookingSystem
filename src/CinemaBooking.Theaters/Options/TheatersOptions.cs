namespace CinemaBooking.Theaters.Options;

/// <summary>
///     Provides a simple surface for configuring the theaters micro-service.
/// </summary>
public class TheatersOptions
{
    /// <summary>
    ///     Gets or sets the type of database to used to manage Theaters data.
    /// </summary>
    /// <remarks>
    ///     This can be useful in various environments to perform testing on
    ///     "non-production" type databases, then switching to "production" type
    ///     databases. Uses the enum <see cref="DbProvider"/>.
    /// </remarks>
    public DbProvider DbProvider { get; set; } = DbProvider.InMemory;

    /// <summary>
    ///     Gets or sets the database connection string.
    /// </summary>
    /// <remarks>
    ///     This is used in conjuction with <see cref="DbProvider"/> to
    ///     establish access to the database. Potentially ignored for certain
    ///     providers.
    /// </remarks>
    public string? DbConnectionString { get; set; }

    /// <summary>
    ///     Gets or sets a flag indicating whether the HTTP endpoints are
    ///     created.
    /// </summary>
    public bool UseEndpoints { get; set; } = false;

    /// <summary>
    ///     Gets or sets the amount of time (in seconds) that a seat will
    ///     remain in <see cref="Seat.OccupancyState.Pending" /> before
    ///     it reverts back to <see cref="Seat.OccupancyState.Vacant"/>.
    /// </summary>
    public int PendingSeatReservationTimeoutInSeconds { get; set; } = 5;
}