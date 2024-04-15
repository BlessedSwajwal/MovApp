using Microsoft.AspNetCore.Http;

namespace Infrastructure.DTOs.Movie;
public class CreateMovieDTO
{
    public CreateMovieDTO(string title, string description, DateOnly releaseDate, IFormFile imageData)
    {
        Title = title;
        Description = description;
        ReleaseDate = releaseDate;
        ImageFile = imageData;
    }

    public string Title { get; set; }
    public string Description { get; set; }
    public DateOnly ReleaseDate { get; set; }

    public IFormFile ImageFile { get; set; }

    public CreateMovieDTO() { }
}
