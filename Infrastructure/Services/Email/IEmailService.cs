using Infrastructure.DTOs.Movie;

namespace Infrastructure.Services.Email;
public interface IEmailService
{
    Task ShareMovie(string emailAddress, MovieDetailDTO movie);
}