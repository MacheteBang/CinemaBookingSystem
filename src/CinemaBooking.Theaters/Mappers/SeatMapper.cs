namespace CinemaBooking.Theaters.Mappers;

public static class SeatMapper
{
    public static SeatResponse ToResponse(this Seat seat)
    {
        return new SeatResponse
        {
            Id = seat.Id,
            Row = seat.Row,
            Number = seat.Number
        };
    }
}