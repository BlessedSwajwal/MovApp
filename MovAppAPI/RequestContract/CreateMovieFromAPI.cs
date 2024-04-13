namespace MovAppAPI.RequestContract;

public record CreateMovieFromAPI(string title, string description, DateOnly ReleaseDate, IFormFile imageFile);
