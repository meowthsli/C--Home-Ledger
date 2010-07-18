using System;
using Meowth.OperationMachine.Domain.Accounts;
using Meowth.OperationMachine.Domain.DomainInfrastructure;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Meowth.OperationMachine.Domain.Entities.Transactions;
using Meowth.OperationMachine.SessionManagement;

namespace Meowth.OperationMachine.CommandHandlers
{
    /// <summary>
    /// Handler for command of creation new transaction from account to account
    /// </summary>
    public class MakeTransactionCommandHandler
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IDomainEventBus _eventBus;

        /// <summary>
        /// Ctor with all dependencies
        /// </summary>
        /// <param name="accountRepository"></param>
        /// <param name="uowFactory"></param>
        /// <param name="eventBus"></param>
        public MakeTransactionCommandHandler(
            IAccountRepository accountRepository,
            IUnitOfWorkFactory uowFactory,
            IDomainEventBus eventBus)
        {
            _uowFactory = uowFactory;
            _eventBus = eventBus;
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// Executes handler
        /// </summary>
        /// <param name="cmd"></param>
        public void Execute(MakeTransactionCommand cmd)
        {
            Account rootAccount = null;
            Func<Account> getRootAccount = () => rootAccount
                ?? (rootAccount = _accountRepository.GetRootAccount());

            using (var uof = _uowFactory.CreateUnitOfWork())
            {
                using (var tx = uof.CreateTransaction())
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

                    tx.Commit();
                }
            }
        }
    }
}
