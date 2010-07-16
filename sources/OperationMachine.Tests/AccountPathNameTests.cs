using System;
using NUnit.Framework;
using Meowth.OperationMachine.Entities.Accounts;

namespace Meowth.OperationMachine.Tests
{
    [TestFixture]
    public class AccountPathNameTests
    {
        [Test]
        public void CannotCreatePathWithFromString()
        {
            var str = "\t  " + Environment.NewLine;
            Assert.Throws<ArgumentException>(
                () => AccountPathName.FromString(str)
                );
        }

        [Test]
        public void CreateOneComponentPath()
        {
            const string str = "expences";
            AccountPathName acc = null;
            Assert.DoesNotThrow(() =>{ acc = AccountPathName.FromString(str); });
            Assert.NotNull(acc);
            var comps = acc.GetNameComponents();
            Assert.AreEqual(1, comps.Length );
            Assert.AreEqual(str, comps[0]);
        }

        [Test]
        public void CreateTwoComponentPath()
        {
            const string str1 = "expences";
            const string str2 = "mobile";
            AccountPathName acc = null;
            Assert.DoesNotThrow(() => { acc = AccountPathName.FromString(str1.ToUpper() + "." + str2); });
            Assert.NotNull(acc);
            var comps = acc.GetNameComponents();
            Assert.AreEqual(2, comps.Length);
            Assert.AreEqual(str1, comps[0]);
            Assert.AreEqual(str2, comps[1]);
            Assert.AreEqual(str1 + "." + str2, acc.ToString());
        }
    }
}
