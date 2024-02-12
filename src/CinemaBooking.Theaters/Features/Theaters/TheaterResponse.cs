using System.Text.Json.Serialization;

namespace CinemaBooking.Theaters.Features.Theaters;

public class TheaterResponse
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Seat>? Seats { get; set; }
}