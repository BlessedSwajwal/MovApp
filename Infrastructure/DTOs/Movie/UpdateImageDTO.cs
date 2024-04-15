using Microsoft.AspNetCore.Http;

namespace Infrastructure.DTOs.Movie;
public record UpdateImageDTO(Guid movieId, IFormFile ImageFile);
