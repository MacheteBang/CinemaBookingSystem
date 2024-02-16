namespace CinemaBooking.Theaters.Mappers;

public static class SeatMapper
{
    public static SeatResponse ToResponse(this Seat seat)
    {
        return new SeatResponse
        {
            Id = seat.Id,
            Occupancy = seat.Occupancy,
            OccupancyStateExpiration = seat.OccupancyStateExpiration,
            IsAvailable = seat.IsAvailable,
            Row = seat.Row,
            Number = seat.Number
        };
    }
}