namespace Infrastructure.Data;
public class Comment
{
    public Guid Id { get; set; }
    public string CommenterId { get; set; }
    public string CommenterName { get; set; }
    public Guid MovieId { get; set; }
    public string Description { get; set; }
    public Comment(Guid id, string description, Guid movieId, string commenterId, string commenterName)
    {
        Id = id;
        Description = description;
        MovieId = movieId;
        CommenterId = commenterId;
        CommenterName = commenterName;
    }

#pragma warning disable CS8618
    private Comment() { }
#pragma warning restore CS8618
}