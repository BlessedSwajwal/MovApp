namespace Infrastructure.DTOs.Movie;
public class CreateMovieDTO
{
    public CreateMovieDTO(string title, string description, DateOnly releaseDate, byte[]? imageData)
    {
        Title = title;
        Description = description;
        ReleaseDate = releaseDate;
        ImageData = imageData;
    }

    public string Title { get; set; }
    public string Description { get; set; }
    public DateOnly ReleaseDate { get; set; }

    public byte[]? ImageData { get; set; }

    public CreateMovieDTO() { }
}
