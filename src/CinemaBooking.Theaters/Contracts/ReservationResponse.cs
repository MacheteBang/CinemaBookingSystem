namespace CinemaBooking.Theaters.Domain;

public class ReservationResponse
{
    public required Guid Id { get; set; }
    public required Guid ShowingId { get; set; }
    public required Guid SeatId { get; set; }
}