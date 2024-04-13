namespace Infrastructure.Data;
public sealed class Movie
{
    public static readonly Movie Empty = new Movie();
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
    public int TotalRates { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public byte[] Image { get; set; }

    //private List<Guid> _commentIds = new();
    //public IReadOnlyList<Guid> CommentIds => _commentIds.AsReadOnly();

    private Movie(
        Guid id,
        string name,
        string description,
        byte[] image,
        int rating,
        int totalRates,
        List<Guid> commentIds,
        DateOnly releaseDate)
    {
        Id = id;
        Name = name;
        Description = description;
        Image = image;
        Rating = rating;
        TotalRates = totalRates;
        ReleaseDate = releaseDate;
    }

    public static Movie CreateNew(string name, string description, byte[] image, DateOnly releaseDate)
    {
        return new(Guid.NewGuid(), name, description, image, 0, 0, new List<Guid>(), releaseDate);

    }

    public static Movie Create(Guid id, string name, string description, byte[] image, int rating, int totalRates, List<Guid> commentIds, DateOnly releaseDate)
    {
        return new(id, name, description, image, rating, totalRates, commentIds, releaseDate);

    }

#pragma warning disable CS8618
    public Movie() { }
#pragma warning restore CS8618
}





