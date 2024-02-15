namespace CinemaBooking.Movies.Mappers;

public static class MovieMapper
{
    public static MovieResponse ToResponse(this Movie movie)
    {
        return new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Description,
            Duration = movie.Duration,
            Genres = movie.Genres
        };
    }
}