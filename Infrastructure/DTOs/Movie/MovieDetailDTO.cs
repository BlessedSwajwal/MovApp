using Infrastructure.Data;

namespace Infrastructure.DTOs.Movie;
public class MovieDetailDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
    public int TotalRates { get; set; }
    public byte[] Image { get; set; }
    public List<Comment> Comments { get; set; }
}
