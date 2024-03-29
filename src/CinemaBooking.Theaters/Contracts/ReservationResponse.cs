namespace CinemaBooking.Theaters.Domain;

public class ReservationResponse
{
    public required Guid Id { get; set; }
    public required Guid SeatId { get; set; }
    public ReservationState State { get; set; }
    public DateTime? PendingExpiresOn { get; set; }
}