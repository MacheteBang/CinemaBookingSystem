namespace CinemaBooking.Theaters.Domain;

public class Reservation : Entity
{
    public required Guid SeatId { get; set; }
}