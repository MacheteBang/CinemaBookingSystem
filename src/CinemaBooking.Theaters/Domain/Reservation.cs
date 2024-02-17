namespace CinemaBooking.Theaters.Domain;

public class Reservation : Entity
{
    public required Guid ShowingId { get; set; }
    public required Guid SeatId { get; set; }
}