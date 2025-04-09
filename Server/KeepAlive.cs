using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;

namespace serverless_signalr_server;

public class KeepAliveFunction
{
    [Function("KeepAlive")]
    public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo timer, FunctionContext context)
    {
        var logger = context.GetLogger("KeepAlive");
        logger.LogInformation($"KeepAlive function executed at: {DateTime.UtcNow}");
    }
}