
namespace serverless_signalr_shared;

public class Class1
{

}


public class NewMessage
{
    public NewMessage(string connectionId, string sender, string message)
    {
        ConnectionId = connectionId;
        Sender = sender;
        Text = message;
    }

    public string ConnectionId { get; }
    public string Sender { get; }
    public string Text { get; }
}



public class NewConnection(string connectionId, string auth)
{
    public string ConnectionId { get; } = connectionId;
    public string Authentication { get; } = auth;
}