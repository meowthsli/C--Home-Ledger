using Meowth.OperationMachine.Domain.Entities;
namespace Meowth.OperationMachine.Domain.Events
{
    /// <summary>
    /// Marker class for event of the domain
    /// </summary>
    /// <typeparam name="TDomainEntity"></typeparam>
    public class DomainEvent<TDomainEntity> : IAnyDomainEvent
        where TDomainEntity : DomainEntity<TDomainEntity>
    {
    }
}
