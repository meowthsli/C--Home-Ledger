using Meowth.OperationMachine.Domain.Entities.Accounts;

namespace Meowth.OperationMachine.Domain.Events.Accounts
{
    public class TurnoverEvent : DomainEvent<Account>
    {
        public TurnoverEvent(TurnoverType type, decimal amount)
        {
            TurnoverType = type;
            Amount = amount;
        }

        public TurnoverType TurnoverType { get; private set; }
        public decimal Amount { get; private set; }
    }
}
