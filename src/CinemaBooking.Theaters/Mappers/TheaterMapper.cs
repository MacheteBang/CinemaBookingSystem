namespace CinemaBooking.Theaters.Mappers;

public static class TheaterMapper
{
    public static TheaterResponse ToResponse(this Theater theater, bool isSummarized = true)
    {
        return new TheaterResponse
        {
            Id = theater.Id,
            Title = theater.Name,
            SeatCount = isSummarized ? theater.Seats.Count : null,
            Seats = isSummarized ? null : theater.Seats
        };
    }
}