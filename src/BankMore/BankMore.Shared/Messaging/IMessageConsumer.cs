namespace BankMore.Shared.Messaging;

public interface IMessageConsumer
{
    Task SubscribeAsync<T>(string topic, Func<T, Task> onMessage, CancellationToken cancellationToken = default);
}
