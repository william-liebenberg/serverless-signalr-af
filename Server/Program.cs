using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.SignalRService;
using Microsoft.Azure.SignalR.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using serverless_signalr_server;
using serverless_signalr_shared;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.AddServerlessHub<ChatHub>();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();

[SignalRConnection("AzureSignalRConnectionString")]
internal partial class ChatHub(IServiceProvider serviceProvider, ILogger<ChatHub> logger) : ServerlessHub<IChatClient>(serviceProvider)
{
    private const string HubName = nameof(ChatHub);
    private readonly ILogger _logger = logger;

    [Function("negotiate")]
    public async Task<HttpResponseData> Negotiate(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var negotiateResponse = await NegotiateAsync();
        var response = req.CreateResponse();
        await response.WriteBytesAsync(negotiateResponse.ToArray());
        return response;
    }

    // This function is triggered by the client connecting to the hub
    // It adds the connection to the counters group and sends the current count
    // The current count is read from a blob before invoking the client methods
    [Function("OnConnected")]
    public Task OnConnected(
        [BlobInput("counter/current.txt")] string counterState,
        [SignalRTrigger(HubName, "connections", "connected")] SignalRInvocationContext invocationContext)
    {
        _logger.LogInformation($"{invocationContext.ConnectionId} has connected");
        Groups.AddToGroupAsync(connectionId: invocationContext.ConnectionId, groupName: "counters");

        return int.TryParse(counterState, out int count) ? Clients.Group("counters").ReceiveCount(count) : Task.CompletedTask;
    }
    
    [Function("OnDisconnected")]
    public void OnDisconnected(
        [SignalRTrigger(HubName, "connections", "disconnected")] SignalRInvocationContext invocationContext)
    {
        _logger.LogInformation($"{invocationContext.ConnectionId} has disconnected");
    }
}

internal partial class ChatHub
{

    // This function is triggered by the client invoking the
    // SendCount method to send the count to all the clients
    // in the counters group.
    // Before completing, the function writes the count to a blob
    [Function("SendCount")]
    [BlobOutput("counter/current.txt")]
    public async Task<string> SendCount(
        [SignalRTrigger(HubName, "messages", "SendCount", "count")]
        SignalRInvocationContext invocationContext, int count)
    {
        await Clients.Group("counters").ReceiveCount(count);
        return count.ToString();
    }
    
    // stream 1000 random values to all the clients in the counters group
    [Function("StreamCount")]
    public async Task StreamCount(
        [SignalRTrigger(HubName, "messages", "StreamCount")]
        SignalRInvocationContext invocationContext)
    {
        var random = new Random();
        for (var i = 0; i < 1000; i++)
        {
            await Clients.Group("counters").ReceiveCount(random.Next(0, 100));
            await Task.Delay(1000);
        }
    }


}