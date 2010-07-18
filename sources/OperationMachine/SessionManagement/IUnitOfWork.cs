using System;
using Meowth.OperationMachine.Domain.Accounts;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Meowth.OperationMachine.Domain.Entities.Transactions;
using Meowth.OperationMachine.Domain.DomainInfrastructure;

namespace Meowth.OperationMachine.SessionManagement
{
    public interface IUnitOfWork : IDisposable
    {
    }

    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateUnitOfWork();
    }

    public class MakeTransactionCommandHandler
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IDomainEventBus _eventBus;

        public MakeTransactionCommandHandler(
            IAccountRepository accountRepository,
            IUnitOfWorkFactory uowFactory,
            IDomainEventBus eventBus)
        {
            _uowFactory = uowFactory;
            _eventBus = eventBus;
            _accountRepository = accountRepository;
        }

        public void Execute(MakeTransactionCommand cmd)
        {
            Account rootAccount = null;
            Func<Account> getRootAccount = () => rootAccount 
                ?? (rootAccount = _accountRepository.GetRootAccount());

            using(_uowFactory.CreateUnitOfWork())
            {
                _eventBus.ClearThreaded();

                var sourcePathName = AccountPathName.FromString(cmd.SourceAccountName);
                var sourceAccount = _accountRepository.FindByPathName(sourcePathName) ??
                                    getRootAccount().CreateSubaccountsTree(sourcePathName);

                var destinationPathName = AccountPathName.FromString(cmd.DestinationAccountName);
                var destinationAccount = _accountRepository.FindByPathName(destinationPathName) ??
                                         getRootAccount().CreateSubaccountsTree(destinationPathName);

                var tran = new Transaction(cmd.Name, sourceAccount, destinationAccount, cmd.Amount);
                tran.Execute();
            }
        }
    }
}
