using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Microsoft.Practices.Unity;
using Meowth.OperationMachine.Domain.DomainInfrastructure;
using Meowth.OperationMachine.Domain.Entities;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;

namespace Meowth.OperationMachine
{
    /// <summary> accounting calculator </summary>
    public class OperationMachine
    {
        /// <summary> In-memory database construction </summary>
        public OperationMachine()
        {
            DomainEntity.SetEventRouter(_container.Resolve<IDomainEventBus>());

            //    .RegisterType<IUnitOfWork, NHUnitOfWork>()
            //    .RegisterType<IAccountRepository, HibernateAccountRepository>()
            //    .RegisterType<INHibernateSessionManager, NHibernateSessionManagerImpl>()
            //    .RegisterInstance(sessionFactory);
        }

        private readonly UnityContainer _container = new UnityContainer();


        //public void MakeTransaction(MakeTransactionCommand cmd)
        //{
        //    Account rootAccount = null;
        //    var accountRepository = ...

        //    Func<Account> getRootAccount = () => rootAccount ?? (rootAccount = accountRepository.GetRootAccount());
            
        //    using(uofFactory.CreateUnitOfWork())
        //    {
        //        var sourcePathName = AccountPathName.FromString(cmd.SourceAccountName);
        //        var sourceAccount = accountRepository.FindByPathName(sourcePathName) ??
        //                            getRootAccount().CreateSubaccountsTree(sourcePathName);

        //        var destinationPathName = AccountPathName.FromString(cmd.DestinationAccountName);
        //        var destinationAccount = accountRepository.FindByPathName(destinationPathName) ??
        //                                 getRootAccount().CreateSubaccountsTree(destinationPathName);

        //        var tran = new Transaction(cmd.Name, sourceAccount, destinationAccount, cmd.Amount);
        //        tran.Execute();
        //    }
        //}
    }

    public class MakeTransactionCommand
    {
        public string Name { get; set; }
        public string SourceAccountName { get; set; }
        public string DestinationAccountName { get; set; }
        public decimal Amount { get; set; }
    }
}
