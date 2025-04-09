using Microsoft.Azure.Functions.Worker;
using serverless_signalr_shared;

namespace serverless_signalr_server;

public static class NewMessageExtensions
{
    public static NewMessage CreateInstance(SignalRInvocationContext invocationContext, string message)
    {
        return new NewMessage(invocationContext.ConnectionId,
            string.IsNullOrEmpty(invocationContext.UserId) ? string.Empty : invocationContext.UserId, message);
    }
}