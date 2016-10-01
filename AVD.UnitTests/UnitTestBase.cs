using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AVD.Common.Logging;
using System.Diagnostics;

namespace AVD.UnitTests
{
    public class UnitTestBase : IDisposable
    {
        private Transaction _transaction;
        [TestInitialize]
        public void BaseTestInitialize()
        {
            if (Transaction.Current != null)
            {
                Assert.Inconclusive();
            }

            var committableTransaction = _transaction as CommittableTransaction;
            
            if (committableTransaction == null)
            {
                committableTransaction = new CommittableTransaction();
                Transaction.Current = committableTransaction;
            } 

            Logger.Instance.Debug("UnitTestBase", "TestCleanup", "Start Transaction - " + Transaction.Current.TransactionInformation.LocalIdentifier);
        }

        [TestCleanup]
        public void BaseTestCleanup()
        {
            Logger.Instance.Debug("UnitTestBase", "TestCleanup", "Rollback - " + Transaction.Current.TransactionInformation.LocalIdentifier);
            Transaction.Current.Rollback();
            Transaction.Current = _transaction;
        }

        public void Dispose()
        {
            // If it enteres here there is a big issue with the tests.
            if (Transaction.Current != null){
                Logger.Instance.Debug("UnitTestBase", "Dispose", "Rollback - " + Transaction.Current.TransactionInformation.LocalIdentifier);
                Transaction.Current.Rollback();
                Transaction.Current = null;
                Assert.Inconclusive("WARNING! SHOULD NOT COME HERE");
            }
        }
    }
}
