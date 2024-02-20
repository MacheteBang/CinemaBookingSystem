namespace CinemaBooking.Theaters.Domain;

public class Reservation : Entity
{
    public required Guid SeatId { get; set; }
    public ReservationState State { get; set; }
    public DateTime? PendingExpiresOn { get; set; }

    public bool IsPending()
    {
        return PendingExpiresOn > DateTime.UtcNow
            && State != ReservationState.Confirmed;
    }

    public bool IsActive()
    {
        return IsPending() || State == ReservationState.Confirmed;
    }
}