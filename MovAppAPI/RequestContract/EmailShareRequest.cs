namespace MovAppAPI.RequestContract;

public record EmailShareRequest(Guid movieId, string to);
