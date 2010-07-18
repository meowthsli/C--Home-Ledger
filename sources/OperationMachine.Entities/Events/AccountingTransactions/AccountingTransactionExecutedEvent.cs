using Meowth.OperationMachine.Domain.Entities.Transactions;

namespace Meowth.OperationMachine.Domain.Events.Transactions
{
    /// <summary>
    /// Business transaction
    /// </summary>
    public sealed class AccountingTransactionExecutedEvent
        : DomainEvent<AccountingTransaction>
    {
        /// <summary>AccountingTransaction executed event .ctor </summary>
        /// <param name="tx"></param>
        public AccountingTransactionExecutedEvent(AccountingTransaction tx)
        {
            AccountingTransaction = tx;
        }

        /// <summary>
        /// Which of transaction executed
        /// </summary>
        public AccountingTransaction AccountingTransaction { get; private set; }
    }
}
