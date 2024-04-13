using Infrastructure.Data;
using Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Implementation.LINQRepository;
public class CommentRepository(ApplicationDbContext dbContext) : ICommentRepository
{
    public async Task AddComment(Comment comment)
    {
        dbContext.Comments.Add(comment);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Comment>> GetCommentsForAMovie(Guid movieId)
    {
        var comments = await dbContext.Comments.Where(c => c.MovieId == movieId).ToListAsync();
        return comments.AsReadOnly();
    }
}
