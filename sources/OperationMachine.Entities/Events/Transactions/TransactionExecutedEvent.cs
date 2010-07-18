using Meowth.OperationMachine.Domain.Entities.Transactions;

namespace Meowth.OperationMachine.Domain.Events.Transactions
{
    /// <summary>
    /// Business transaction
    /// </summary>
    public sealed class TransactionExecutedEvent
        : DomainEvent<Transaction>
    {
        /// <summary>Transaction executed event .ctor </summary>
        /// <param name="tx"></param>
        public TransactionExecutedEvent(Transaction tx)
        {
            Transaction = tx;
        }

        /// <summary>
        /// Which of transaction executed
        /// </summary>
        public Transaction Transaction { get; private set; }
    }
}
