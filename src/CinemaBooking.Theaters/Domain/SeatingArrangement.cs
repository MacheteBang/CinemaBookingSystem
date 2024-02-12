// TODO: Where should this live?
namespace CinemaBooking.Theaters.Domain;

public class SeatingArrangement
{
    public static readonly SeatingArrangement Default = new();

    private SeatingArrangement() { }

    public static List<Seat> GetSeatingArrangement(SeatingArrangement seatingArrangement)
    {
        return seatingArrangement switch
        {
            _ => GetDefault()
        };
    }

    public static List<Seat> GetSeatingArrangement(string seatingArrangement)
    {
        return seatingArrangement.ToUpper() switch
        {
            "DEFAULT" => GetSeatingArrangement(Default),
            _ => throw new Exception("Seating arrangement not found")
        };
    }


    private static List<Seat> GetDefault()
    {
        List<Seat> seats = [];

        // The first 5 rows have 15 seats
        for (char letter = 'a'; letter <= 'e'; letter++)
        {
            for (ushort i = 1; i <= 15; i++)
            {
                seats.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Row = letter.ToString(),
                    SeatNumber = i
                });
            }
        }

        // The remaining rows have 20 seats
        for (char letter = 'f'; letter <= 'e'; letter++)
        {
            for (ushort i = 1; i <= 20; i++)
            {
                seats.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Row = letter.ToString(),
                    SeatNumber = i
                });
            }
        }

        return seats;
    }
}