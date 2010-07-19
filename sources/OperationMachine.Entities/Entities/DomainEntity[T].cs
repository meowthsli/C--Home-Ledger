namespace Meowth.OperationMachine.Domain.Entities
{
    /// <summary> Typed domain entity </summary>
    public abstract class DomainEntity<TEntity> : IAnyDomainEntity
        where TEntity : DomainEntity<TEntity>
    {
    }
}
