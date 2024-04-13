using Infrastructure.Data;

namespace MovAppAPI.RequestContract;

public record UpdateNameAndDesc(Guid movieId, string title, string description, List<Comment> comments, int rating, int totalRates, byte[] image_data);
