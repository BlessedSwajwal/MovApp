using Infrastructure.Data;
using Infrastructure.Notification;
using Infrastructure.Persistence.Repositories.Interfaces;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services.Implementation;


public class CommentService(ICommentRepository _commentRepository, IHubContext<CommentUpdateHub> commentHub) : ICommentService
{
    public async Task PostComment(string commentText, Guid movieId, string commenterId, string commenterName)
    {
        var comment = new Comment(Guid.NewGuid(), commentText, movieId, commenterId, commenterName);
        await _commentRepository.AddComment(comment);
        await commentHub.Clients.Groups($"movie-{comment.MovieId}").SendAsync("ReceiveComment", comment);
    }
}
