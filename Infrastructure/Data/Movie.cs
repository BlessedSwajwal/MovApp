namespace Infrastructure.Data;
public sealed class Movie
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
    public int TotalRates { get; set; }
    public byte[] Image { get; set; }

    private List<Guid> _commentIds = new();
    public IReadOnlyList<Guid> CommentIds => _commentIds.AsReadOnly();

    public Movie(string name, string description, byte[] image)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Image = image;
        Rating = 0;
        TotalRates = 0;
        Console.WriteLine(Id);

    }

#pragma warning disable CS8618
    public Movie() { }
#pragma warning restore CS8618
}

public class Comment
{
    public Guid Id { get; set; }
    public Guid CommenterId { get; set; }
    public string CommenterName { get; set; }
    public Guid MovieId { get; set; }
    public string Description { get; set; }
    public Comment(Guid id, string description, Guid movieId, Guid commenterId, string commenterName)
    {
        Id = id;
        Description = description;
        MovieId = movieId;
        CommenterId = commenterId;
        CommenterName = commenterName;
    }

#pragma warning disable CS8618
    private Comment() { }
#pragma warning restore CS8618
}

