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
    public string ImagePath { get; set; }


    private Movie(
        Guid id,
        string name,
        string description,
        string imagePath,
        int rating,
        int totalRates,
        List<Guid> commentIds,
        DateOnly releaseDate)
    {
        Id = id;
        Name = name;
        Description = description;
        ImagePath = imagePath;
        Rating = rating;
        TotalRates = totalRates;
        ReleaseDate = releaseDate;
    }

    public static Movie CreateNew(string name, string description, string imagePath, DateOnly releaseDate)
    {
        return new(Guid.NewGuid(), name, description, imagePath, 0, 0, new List<Guid>(), releaseDate);
    }

    public static Movie Create(Guid id, string name, string description, string imagePath, int rating, int totalRates, List<Guid> commentIds, DateOnly releaseDate)
    {
        return new(id, name, description, imagePath, rating, totalRates, commentIds, releaseDate);

    }

#pragma warning disable CS8618
    public Movie() { }
#pragma warning restore CS8618
}





