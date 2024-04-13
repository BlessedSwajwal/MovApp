using Infrastructure.Data;
using Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Implementation.SQLRepository;
public class SQLCommentRepository(ApplicationDbContext dbContext) : ICommentRepository
{
    public async Task AddComment(Comment comment)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync($"EXEC AddComments {comment.Id}, {comment.CommenterId}, {comment.MovieId}, {comment.CommenterName}, {comment.Description}");
    }

    public async Task<IReadOnlyList<Comment>> GetCommentsForAMovie(Guid movieId)
    {
        var comments = await dbContext.Comments.FromSqlInterpolated(
    $@"EXEC GetCommentsForAMovie
                    {movieId}").ToListAsync();
        return comments;
    }
}
