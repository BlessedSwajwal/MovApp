using Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;
using Quartz;

namespace Infrastructure.Services.Implementation;
public class NotificationJob(IHubContext<NotificationHub> hubContext, ApplicationDbContext dbContext) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await Console.Out.WriteLineAsync("Firing the job");
        var movies = dbContext.Movies.Take(3).ToList();
        await hubContext.Clients.All.SendAsync("ReceiveNotification", movies);
    }
}
