using Meowth.OperationMachine.Domain.Events;

namespace Meowth.OperationMachine.Domain.DomainInfrastructure
{
    public interface IRouteEvents
    {
        void Route(IAnyDomainEvent ev);
    }
}
