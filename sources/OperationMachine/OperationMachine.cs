using System;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Meowth.OperationMachine.Domain.Accounts;
using Meowth.OperationMachine.Domain;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using NHibernate;
using System.Collections.Generic;

namespace Meowth.OperationMachine
{
    /// <summary> accounting calculator </summary>
    public class OperationMachine
    {
        /// <summary> In-memory database construction </summary>
        public OperationMachine()
        {
            _sessionFactory = Fluently.Configure()
               .Mappings(m => m.AutoMappings.Add(AutoMap
                   .AssemblyOf<Account>()
                   .Override<Account>(a => a.IgnoreProperty(x => x.Turnover))
                   .Override<Account>(a => a.IgnoreProperty(x => x.Balance))))
               .Database(SQLiteConfiguration.Standard
                   .ConnectionString("Data Source=:memory:;Version=3;BinaryGUID=False;New=True;")
                   .Driver("NHibernate.Driver.SQLite20Driver")
                   .Dialect("NHibernate.Dialect.SQLiteDialect")
                   .QuerySubstitutions("true=1;false=0")
                   .ProxyFactoryFactory("NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu"))
               .BuildConfiguration()
               .BuildSessionFactory();

            using(var session = _sessionFactory.OpenSession())
            using(var tx = session.BeginTransaction())
            {
                Initialize(session);
                tx.Commit();
            }
        }

        private void Initialize(ISession session)
        {
            var acc = new Account("root");
            session.Save(acc);
        }

        public interface IDateTimeService
        {
            DateTime Now { get; }
        }
        
        public Account GetAccount(AccountPathName pathName)
        {
            return null;
        }

        public void Reset()
        {
            // TODO:
        }

        public IEnumerable<Account> GetAccounts()
        {
            yield break;
        }

        private readonly ISessionFactory _sessionFactory;
    }
}
