namespace CinemaBooking.Theaters.Mappers;

public static class ReservationMapper
{
    public static ReservationResponse ToResponse(this Reservation reservation)
    {
        return new ReservationResponse
        {
            Id = reservation.Id,
            SeatId = reservation.SeatId
        };
    }
}