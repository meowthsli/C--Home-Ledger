using Meowth.OperationMachine.Domain.Entities;
namespace Meowth.OperationMachine.Domain.Events
{
    /// <summary> Event of creation of entity </summary>
    /// <typeparam name="T">Type of created entity</typeparam>
    public class EntityCreatedEvent<T> : DomainEvent<T>
        where T : DomainEntity<T>
    {
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="subject"></param>
        public EntityCreatedEvent(T subject)
        {
            Subject = subject;
        }

        /// <summary>
        /// Entity that was created
        /// </summary>
        public T Subject { get; private set; }
    }
}
