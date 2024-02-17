namespace CinemaBooking.Theaters.Domain;

public class Seat : Entity
{
    public required string Row { get; set; }
    public required ushort Number { get; set; }
}