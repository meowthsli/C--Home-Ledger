using Meowth.OperationMachine.Domain.Entities.Accounts;

namespace Meowth.OperationMachine.Domain.Events
{
    public class EntityLifecycleEvent<T> : DomainEvent<T>
    {
        public EntityLifecycleEvent(T subject, EntityLifecyclePhase lifeCycle)
        {
            Subject = subject;
            EntityLifecycle = lifeCycle;
        }

        public T Subject { get; private set; }

        public EntityLifecyclePhase EntityLifecycle { get; private set; }
    }
}
