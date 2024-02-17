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
            Showtime = showing.Showtime
        };
    }
}