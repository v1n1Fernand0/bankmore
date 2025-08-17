namespace BankMore.Shared.Domain;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
