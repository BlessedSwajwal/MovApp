namespace Infrastructure.Services.Interfaces;
public interface ICommentService
{
    Task PostComment(string commentText, Guid movieId, string commenterId, string commenterName);
}

