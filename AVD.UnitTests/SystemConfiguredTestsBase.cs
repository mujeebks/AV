using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AVD.Common.Caching;
using AVD.Core;
using AVD.Core.Auth;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Enums;
using AVD.DataAccessLayer.Models;
using AVD.DataAccessLayer.Repositories;
using AVD.UnitTests.Common.Configuration;

namespace AVD.UnitTests
{
    /// <summary>
    /// Subclassing from this will ensure that the system has been configured
    /// properly. It is the equlivant of what should be in the Application Startup code
    /// in Global.asax. 
    /// 
    /// In addition for each test this will occur:
    /// - Start transaction
    /// - Impersonate an agent
    /// - Clear cache
    /// </summary>
    public abstract class SystemConfiguredTestsBase : UnitTestBase
    {
        public int? AuthenticatedAgentId { get; private set; }
        public int? AuthenticatedUserId { get; private set; }
        
        private ImpersonatedAccount ImpersonatedAccount { get; set; }

        protected SystemConfiguredTestsBase()
        {
            RunSystemConfiguration();
        }

        public static void RunSystemConfiguration()
        {
            if (!AutoMapperConfiguration.Initialized)
                AutoMapperConfiguration.Configure();
            if (!CurrencyConfiguration.Initialized)
                CurrencyConfiguration.Configure();
        }

        [TestInitialize]
        public void ImpersonatedAccountInitialize()
        {
            ServiceLocator.CachingManager.Clear();
            ImpersonateAgent();
        }

        [TestCleanup]
        public void ImpersonatedAccountCleanup()
        {
            if(ImpersonatedAccount != null)
            {
                ImpersonatedAccount.Dispose();
            }

            ImpersonatedAccount = null;

            ServiceLocator.CachingManager.Clear();
        }

        /// <summary>
        /// This will impersonate the agent with the email address defined inside the UserTestData file.
        /// 
        /// This user must have an agent and user account already established for this impersonation to work
        /// </summary>
        private bool ImpersonateAgent()
        {
            int? agentId;
            int? userId;

            var user = ImpersonateAgent(out agentId, out userId);

            AuthenticatedAgentId = agentId;
            AuthenticatedUserId = userId;
            
            return true;
        }

        /// <summary>
        /// This will impersonate the agent with the email address defined inside the UserTestData file.
        /// 
        /// This user must have an agent and user account already established for this impersonation to work
        /// </summary>
        private User ImpersonateAgent(out int? agentId, out int? userId)
        {
            var agentRepo = RepositoryFactory.Get<User>();

            var userName = TestConfigSettings.Instance().UnitTestsImpersonatedAgentUsername;

            if(userName == null)
                throw new ApplicationException("Missing config value - UnitTestsImpersonatedAgentUsername");

            var agent =
                agentRepo.Get(a => a.Username == userName).SingleOrDefault();

            if (agent == null)
                Assert.Inconclusive("There is no valid agent with email " + userName + " to impersonate in the DB");

            ImpersonatedAccount = new ImpersonatedAccount(TestConfigSettings.Instance().UnitTestsImpersonatedAgentUsername);

            agentId = agent.UserId;
            userId = agent.UserId;
            
            return agent;
        }
    }
}
