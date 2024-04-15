namespace Infrastructure.DTOs.Movie;
public class MovieListDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
    public int TotalRates { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public string ImagePath { get; set; }
}
