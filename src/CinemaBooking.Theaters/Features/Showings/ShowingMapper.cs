namespace CinemaBooking.Theaters.Features.Showings;

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
            Seats = showing.Seats
        };
    }
}