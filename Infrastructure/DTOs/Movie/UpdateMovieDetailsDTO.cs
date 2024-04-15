namespace Infrastructure.DTOs.Movie;
public record UpdateMovieDetailsDTO(Guid MovieId, string name, string description, DateOnly releaseDate);