using System.Data.SQLite;
using System.Diagnostics;
using Meowth.OperationMachine.Entities.Accounts;
using NUnit.Framework;
using FluentNHibernate.Cfg;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg.Db;

namespace Meowth.OperationMachine.Tests
{
    [TestFixture]
    public class DbTests
    {
        [Test]
        public void EmptyDbCreatedAndOpened()
        {
            using (var conn = new SQLiteConnection(
                "Data Source=:memory:;Version=3;BinaryGUID=False;New=True;"))
            {
                conn.Open();

                conn.Close();
            }
        }

        [Test]
        public void FluentltyConfigureDb()
        {
            var sf = Fluently.Configure()
                .Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<Transaction>))
                .Database(SQLiteConfiguration.Standard
                    .ConnectionString("Data Source=:memory:;Version=3;BinaryGUID=False;New=True;")
                    .Driver("NHibernate.Driver.SQLite20Driver")
                    .Dialect("NHibernate.Dialect.SQLiteDialect")
                    .QuerySubstitutions("true=1;false=0")
                    .ProxyFactoryFactory("NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu"))
                .BuildConfiguration()
                .BuildSessionFactory();

            foreach (var acm in sf.GetAllClassMetadata())
                Trace.WriteLine(acm.Key);

            foreach (var acm in sf.GetAllCollectionMetadata())
                Trace.WriteLine(acm.Key);

            using(var sess = sf.OpenSession())
            {
            }
        }
    }
}
