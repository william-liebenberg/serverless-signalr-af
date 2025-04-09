namespace serverless_signalr_shared;

public interface IChatClient
{
    Task ReceiveCount(int count);
}