using System;
using Meowth.OperationMachine.Commands;
using Meowth.OperationMachine.Domain.Accounts;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Meowth.OperationMachine.Domain.Entities.Transactions;

namespace Meowth.OperationMachine.CommandHandlers
{
    /// <summary>
    /// Handler for command of creation new transaction from account to account
    /// </summary>
    internal class MakeTransactionCommandHandler : CommandHandler<MakeAccountingTransactionCommandDTO>
    {
        private readonly IAccountRepository _accountRepository;

        /// <summary>
        /// Ctor with all dependencies
        /// </summary>
        /// <param name="accountRepository"></param>
        public MakeTransactionCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public override void Handle(MakeAccountingTransactionCommandDTO cmd)
        {
            Account rootAccount = null;
            Func<Account> getRootAccount = () => rootAccount
                ?? (rootAccount = _accountRepository.GetRootAccount());

            var sourcePathName = AccountPathName.FromString(cmd.SourceAccountName);
            var sourceAccount = _accountRepository.FindByPathName(sourcePathName) ??
                                getRootAccount().CreateSubaccountsTree(sourcePathName);

            var destinationPathName = AccountPathName.FromString(cmd.DestinationAccountName);
            var destinationAccount = _accountRepository.FindByPathName(destinationPathName) ??
                                     getRootAccount().CreateSubaccountsTree(destinationPathName);

            var tran = new AccountingTransaction(cmd.Name, sourceAccount, destinationAccount, cmd.Amount);
            tran.Execute();
        }
    }
}
