using System;
using Meowth.OperationMachine.Domain.Entities.Transactions;
namespace Meowth.OperationMachine.Domain.DomainInfrastructure.Repository
{
    public interface IAccountingTransactionRepository
    {
        AccountingTransaction GetById(Guid id);
    }
}
