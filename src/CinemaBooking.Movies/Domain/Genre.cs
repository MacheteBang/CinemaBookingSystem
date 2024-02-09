namespace CinemaBooking.Movies;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Genre
{
    Action,
    Comedy,
    Drama,
    Adventure,
    ScienceFiction,
    Thriller,
    Horror,
    Romance,
    Fantasy,
    Animation
}