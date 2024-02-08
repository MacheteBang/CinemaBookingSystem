namespace CinemaBooking.Movies;

public class Genre
{
    public static readonly Genre Action = new("Action");
    public static readonly Genre Adventure = new("Adventure");
    public static readonly Genre Animation = new("Animation");
    public static readonly Genre Comedy = new("Comedy");
    public static readonly Genre Crime = new("Crime");
    public static readonly Genre Drama = new("Drama");
    public static readonly Genre Fantasy = new("Fantasy");
    public static readonly Genre Horror = new("Horror");
    public static readonly Genre Mystery = new("Mystery");
    public static readonly Genre Romance = new("Romance");
    public static readonly Genre SciFi = new("Science Fiction");
    public static readonly Genre Thriller = new("Thriller");
    public static readonly Genre Documentary = new("Documentary");
    public static readonly Genre Family = new("Family");
    public static readonly Genre Music = new("Music");
    public static readonly Genre Musical = new("Musical");
    public static readonly Genre War = new("War");
    public static readonly Genre Western = new("Western");
    public static readonly Genre History = new("History");
    public static readonly Genre Sport = new("Sport");

    private readonly string _value;

    private Genre(string value)
    {
        _value = value;
    }

    public override string ToString()
    {
        return _value;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Genre other = (Genre)obj;
        return _value == other._value;
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
}
