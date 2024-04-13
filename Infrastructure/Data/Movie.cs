﻿namespace Infrastructure.Data;
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

public class Comment
{
    public Guid Id { get; set; }
    public string CommenterId { get; set; }
    public string CommenterName { get; set; }
    public Guid MovieId { get; set; }
    public string Description { get; set; }
    public Comment(Guid id, string description, Guid movieId, string commenterId, string commenterName)
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

public class Ratings
{
    public Guid Id;
    public Guid MovieId { get; set; }
    public string RatersId { get; set; }
    public int Rating { get; set; }

    public Ratings(Guid movieId, string ratersId, int rating)
    {
        Id = Guid.NewGuid();
        RatersId = ratersId;
        Rating = rating;
        MovieId = movieId;
    }
}

