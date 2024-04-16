using Infrastructure.Data;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Notification;
public class NotificationHub(ApplicationDbContext dbContext) : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Get user information from connection (if applicable)
        //var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        //get movies 
        var movies = dbContext.Movies.Where(m => m.ReleaseDate > DateOnly.FromDateTime(DateTime.Now)).ToList();
        // Call method to send notification to connected user
        await SendConnectedNotifcation(movies);

        await base.OnConnectedAsync();
    }
    public async Task SendNotification(List<Movie> notificationMovies)
    {
        await Clients.All.SendAsync("ReceiveNotification", notificationMovies);
    }

    private async Task SendConnectedNotifcation(List<Movie> movies)
    {
        // Send notification to the connected user
        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveNotification", movies);
    }
}
