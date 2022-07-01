using Grpc.Core;
using notification;

namespace notification.Services;

public class NotificationService : Notification.NotificationBase
{
    private readonly ILogger<NotificationService> _logger;
    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public override Task<NotifyReply> NotifyEvent(NotifyRequest request, ServerCallContext context)
    {
        _logger.LogDebug(request.ToString());
        Console.WriteLine("Event: " + request.EventName + " occured with parameters: \n" + request.Params.ToString());
        return Task.FromResult(new NotifyReply
        {
            Message = "Event " + request.EventName + " received."
        });
    }
}
