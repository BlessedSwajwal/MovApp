namespace Infrastructure.Data;
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
