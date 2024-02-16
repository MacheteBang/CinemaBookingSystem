namespace CinemaBooking.Theaters.Mappers;

public static class ShowingMapper
{
    public static ShowingResponse ToResponse(this Showing showing)
    {
        return new ShowingResponse
        {
            Id = showing.Id,
            TheaterId = showing.TheaterId,
            MovieId = showing.MovieId,
            Showtime = showing.Showtime,
            SeatSummary = showing.Seats // FIXME: Logic for Summarizing seats should not be in a mapper.
                .GroupBy(s => s.IsAvailable)
                .ToDictionary(g => g.Key, g => Convert.ToUInt32(g.Count()))
        };
    }
}