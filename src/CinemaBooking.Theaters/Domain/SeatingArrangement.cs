// TODO: Where should this live?
namespace CinemaBooking.Theaters.Domain;

public class SeatingArrangement
{
    private static readonly Dictionary<string, SeatingArrangement> _dictionary = new()
    {
        {"Default", new(GetDefault, "Default")}
    };

    public static readonly SeatingArrangement Default = _dictionary["Default"];

    public string Name { get; init; }
    private readonly Func<List<Seat>> _seatFiller;

    private SeatingArrangement(Func<List<Seat>> seatFiller, string name)
    {
        Name = name;
        _seatFiller = seatFiller;
    }

    public static SeatingArrangement GetSeatingArrangement(string name)
    {
        return _dictionary[name];
    }

    public List<Seat> GetSeats()
    {
        return _seatFiller.Invoke();
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

    public static implicit operator string(SeatingArrangement seatingArrangement) => seatingArrangement.Name;
    public static implicit operator SeatingArrangement(string name) => GetSeatingArrangement(name);
}