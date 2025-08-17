namespace BankMore.Shared.Messaging;

public interface IMessageProducer
{
    Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default);
}
