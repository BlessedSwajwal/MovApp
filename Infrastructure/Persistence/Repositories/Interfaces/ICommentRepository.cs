using Infrastructure.Data;

namespace Infrastructure.Persistence.Repositories.Interfaces;
public interface ICommentRepository
{
    Task AddComment(Comment comment);
    Task<IReadOnlyList<Comment>> GetCommentsForAMovie(Guid movieId);
}

