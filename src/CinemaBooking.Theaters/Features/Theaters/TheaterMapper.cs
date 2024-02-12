namespace CinemaBooking.Theaters.Features.Theaters;

public static class TheaterMapper
{
    public static TheaterResponse ToResponse(this Theater theater)
    {
        return new TheaterResponse
        {
            Id = theater.Id,
            Title = theater.Name,
            SeatingArrangement = theater.SeatingArrangement
        };
    }
}