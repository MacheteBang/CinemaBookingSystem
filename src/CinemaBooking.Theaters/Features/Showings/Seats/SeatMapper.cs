namespace CinemaBooking.Theaters.Features.Showings.Seats;

public static class SeatMapper
{
    public static SeatResponse ToResponse(this Seat seat)
    {
        return new SeatResponse
        {
            Id = seat.Id,
            Occupancy = seat.Occupancy,
            Row = seat.Row,
            SeatNumber = seat.SeatNumber
        };
    }
}