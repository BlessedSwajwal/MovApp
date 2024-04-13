using Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Notification;
public class NotificationHub : Hub
{
    public async Task SendNotification(List<Movie> notificationMovies)
    {
        await Clients.All.SendAsync("ReceiveNotification", notificationMovies);
    }
}
