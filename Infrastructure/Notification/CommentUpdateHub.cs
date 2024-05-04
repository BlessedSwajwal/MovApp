using Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Notification;
public class CommentUpdateHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        string? movieId = Context.GetHttpContext()?.Request.Query["movieId"];

        if (movieId == null) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"movie-{movieId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        string movieId = Context.GetHttpContext().Request.Query["movieId"];
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"movie-{movieId}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendComment(Comment comment, Guid movieId)
    {
        await Clients.Group($"movie-{movieId}").SendAsync("ReceiveComment", comment);
    }
}
