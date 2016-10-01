using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AVD.Common.Logging;

namespace TE.UnitTests
{
    public class TransactionScopeHelper
    {
        private Transaction _transaction;

        private TestContext TestContext { get; set; }

        public TransactionScopeHelper(TestContext testContext)
        {
            TestContext = testContext;
        }

        private TransactionScope ts;

        public void Init()
        {
            //if(Transaction.Current == null)
            //    Logger.Instance.Debug("TransactionScopeHelper", "Init", null);
            //else
            //    Logger.Instance.Debug("TransactionScopeHelper", "Init", Transaction.Current.TransactionInformation.LocalIdentifier + " " + Transaction.Current.TransactionInformation.Status);

           // _transaction = Transaction.Current;

            //ts = new TransactionScope();
            //ts.

            //if (_transaction != null){
            //    if (_transaction.TransactionInformation.Status == TransactionStatus.Aborted)
            //    {
            //        Trace.WriteLine(String.Format("Init transaction : {0}, {1}",
            //            _transaction.TransactionInformation.Status,
            //            _transaction.TransactionInformation.LocalIdentifier));

            //        Assert.Inconclusive("Transaction issue - " + _transaction.TransactionInformation.LocalIdentifier);
            //        _transaction.Rollback();
            //    }
            //}
            //else
            //    Trace.WriteLine("Init transaction : NULL");

            //var committableTransaction = _transaction as CommittableTransaction;
            //if (committableTransaction == null)
            //{
            //    committableTransaction = new CommittableTransaction();
            //    Transaction.Current = committableTransaction;
            //}
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run.
        /// </summary>
        public void Rollback()
        {
            if (Transaction.Current == null)
                Logger.Instance.Debug("TransactionScopeHelper", "Rollback", "");
            else
                Logger.Instance.Debug("TransactionScopeHelper", "Rollback", Transaction.Current.TransactionInformation.LocalIdentifier + " " + Transaction.Current.TransactionInformation.Status);



        // ts.Dispose();   
            //if (Transaction.Current != null)
            //    Trace.WriteLine(String.Format("Rollback transaction : {0}, {1}", Transaction.Current.TransactionInformation.Status,
            //    Transaction.Current.TransactionInformation.LocalIdentifier));
            //else
            //    Trace.WriteLine("Rollback transaction : NULL");

            //Transaction.Current.Rollback();
            ////Transaction.Current = _transaction;
        }
    }
}
