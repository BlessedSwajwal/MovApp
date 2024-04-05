using Infrastructure.DTOs.Movie;
using System.Security.Claims;

namespace MovApp.Models;

public class MovieDetailViewModel
{
    public MovieDetailViewModel(ClaimsPrincipal user, MovieDetailDTO movieDetailDTO)
    {
        User = user;
        this.movieDetailDTO = movieDetailDTO;
    }

    public ClaimsPrincipal User { get; set; }
    public MovieDetailDTO movieDetailDTO { get; set; }
}
