using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Meowth.OperationMachine.CommandHandlers;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Meowth.OperationMachine.RepositoryImplementation;
using Meowth.OperationMachine.SessionManagement;
using Meowth.OperationMachine.SessionManagement.NHibernate;
using Microsoft.Practices.Unity;
using Meowth.OperationMachine.Domain.DomainInfrastructure;
using Meowth.OperationMachine.Domain.Entities;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using NHibernate.Tool.hbm2ddl;
using System.IO;
using Meowth.OperationMachine.Commands;

namespace Meowth.OperationMachine
{
    /// <summary> accounting calculator </summary>
    public class OperationMachine
    {
        /// <summary> In-memory database construction </summary>
        public OperationMachine()
        {
        }

        public OperationMachine(string fileName)
        {
            var exist = File.Exists(fileName);
            //log4net.Config.XmlConfigurator.Configure();

            var cfg = Fluently.Configure()
               .Mappings(m => m.FluentMappings.AddFromAssemblyOf<AccountMapping>())
               .Database(SQLiteConfiguration.Standard
                   .ShowSql()
                   .ConnectionString(string.Format("Data Source={0};Version=3;BinaryGUID=True;New=True;Pooling=True;Max Pool Size=1;", fileName))
                   .Driver("NHibernate.Driver.SQLite20Driver")
                   .Dialect("NHibernate.Dialect.SQLiteDialect")
                   .QuerySubstitutions("true=1;false=0")
                   .ProxyFactoryFactory("NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu"))
               .BuildConfiguration();

            var sessionFactory = cfg
               .BuildSessionFactory();

            _container
                .RegisterInstance(sessionFactory)
                .RegisterType<IDomainEventBus, DomainEventBusGate>()

                // repositories
                .RegisterType<IAccountRepository, HibernateAccountRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IAccountingTransactionRepository, HibernateAccountingTransactionRepository>(new ContainerControlledLifetimeManager())

                // session management
                .RegisterType<IHibernateSessionManager, HibernateSessionManagerImpl>(new ContainerControlledLifetimeManager())
                .RegisterType<IUnitOfWorkFactory, HibernateUnitOfWorkFactory>(new ContainerControlledLifetimeManager());

            _container.AddExtension(new RegistrationExtension());

            if (!exist)
            {
                new SchemaExport(cfg).Execute(true, true, false);
                using (var uow = _container.Resolve<IUnitOfWorkFactory>().CreateUnitOfWork())
                {
                    using (var tx = uow.CreateTransaction())
                    {
                        if (_container.Resolve<IAccountRepository>().GetRootAccount() == null)
                        {
                            new Account("root");
                            tx.Commit();
                        }
                    }
                }
            }
        }

        public void ExecuteCommand(MakeAccountingTransactionCommandDTO dto, IUnitOfWork uow)
        {
            using (var tx = uow.CreateTransaction())
            {
                var hndl = _container.Resolve<MakeTransactionCommandHandler>();
                hndl.Handle(dto);
                tx.Commit();
            }
        }

        private readonly UnityContainer _container = new UnityContainer();

        public IUnitOfWork CreateUnitOfWork()
        {
            return _container.Resolve<IUnitOfWorkFactory>().CreateUnitOfWork();
        }
    }
}
