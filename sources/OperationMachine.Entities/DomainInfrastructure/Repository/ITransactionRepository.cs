using System;
using Meowth.OperationMachine.Domain.Entities.Transactions;
namespace Meowth.OperationMachine.Domain.DomainInfrastructure.Repository
{
    public interface ITransactionRepository
    {
        AccountingTransaction GetById(Guid id);
    }
}
