using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp1;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using serverless_signalr_shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:7071/api") });
 
builder.Services.AddScoped<HubConnection>(sp =>  
{  
    var navigationManager = sp.GetRequiredService<NavigationManager>();  
    return new HubConnectionBuilder()  
        .WithUrl(new Uri("http://localhost:7071/api"))  
        .WithAutomaticReconnect()  
        .Build();  
});  

// builder.Services.AddScoped<HubActionBinder>();

await builder.Build().RunAsync();  
  
 
// public class HubActionBinder  
// {  
//     private readonly HubConnection _hubConnection;  
//  
//     public HubActionBinder(HubConnection hubConnection)  
//     {  
//         _hubConnection = hubConnection;  
//     }  
//  
//     public async Task Bind()  
//     {  
//         _hubConnection.On<NewMessage>(methodName: nameof(IChatClient.newMessage), handler: ReceiveNewMessage);  
//  
//         if (_hubConnection.State == HubConnectionState.Disconnected)  
//         {  
//             await _hubConnection.StartAsync();  
//         }  
//         
//         
//         // YUCKY
//         await _hubConnection.InvokeAsync(methodName: "newMessage", "Hello World");
//         
//         // YUMMMY
//         await _hubConnection.InvokeAsync(methodName: nameof(IChatClient.newMessage), "Hello World");
//     }  
//
//     public Task ReceiveNewMessage(NewMessage message)  
//     {  
//         return Task.CompletedTask;  
//     }  
// }