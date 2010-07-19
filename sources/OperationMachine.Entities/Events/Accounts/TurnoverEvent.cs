using Meowth.OperationMachine.Domain.Entities.Transactions;
namespace Meowth.OperationMachine.Domain.Events.Accounts
{
    public class TurnoverEvent : DomainEvent<AccountingTransaction>
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
