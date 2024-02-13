namespace CinemaBooking.Theaters.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SeatOccupancy
{
    Vacant,
    Occupied
}