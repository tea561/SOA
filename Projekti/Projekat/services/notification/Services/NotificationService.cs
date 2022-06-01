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
        Console.WriteLine("Event: " + request.EventName + " occured with parameter " + request.ParameterName + " = " + request.ParameterValue);
        return Task.FromResult(new NotifyReply
        {
            Message = "Event " + request.EventName + " received."
        });
    }
}
