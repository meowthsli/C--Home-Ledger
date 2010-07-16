using Meowth.OperationMachine.Domain.Entities.Transactions;

namespace Meowth.OperationMachine.Domain.Events.Transactions
{
    /// <summary>
    /// BVTransaction
    /// </summary>
    public sealed class TransactionExecutedEvent
        : DomainEvent<Transaction>
    {
        public TransactionExecutedEvent(Transaction tx)
        {
            Transaction = tx;
        }

        public Transaction Transaction { get; private set; }
    }
}
