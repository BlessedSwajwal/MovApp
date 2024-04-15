using Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;
using Quartz;

namespace Infrastructure.Notification;
public class NotificationJob(IHubContext<NotificationHub> hubContext, ApplicationDbContext dbContext) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await Console.Out.WriteLineAsync("Firing the job");
        var movies = dbContext.Movies.Where(m => m.ReleaseDate > DateOnly.FromDateTime(DateTime.Now)).Take(3).ToList();
        Notifications.NotificationsMovies = movies;
        await hubContext.Clients.All.SendAsync("ReceiveNotification", movies);
    }
}
