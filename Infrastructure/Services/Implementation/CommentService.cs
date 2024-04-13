using Infrastructure.Data;
using Infrastructure.Persistence.Repositories.Interfaces;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services.Implementation;


public class CommentService(ICommentRepository _commentRepository) : ICommentService
{
    public async Task PostComment(string commentText, Guid movieId, string commenterId, string commenterName)
    {
        var comment = new Comment(Guid.NewGuid(), commentText, movieId, commenterId, commenterName);
        await _commentRepository.AddComment(comment);
    }
}
