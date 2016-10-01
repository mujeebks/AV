using AVD.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using AVD.Common;
using AVD.Common.Logging;
using AVD.DataAccessLayer.Repositories;
using AVD.Core;

namespace AVD.Core.Planning
{
    /// <summary>
    /// Provides services relating to creating, modifying or deleteing planning entities and other information.
    /// </summary>
   public partial class PlanningManager
    {
        protected IRepository<MD_EntityType> EntityTypeRepository;
        private IPersistentCacheManager DbCacheManager { get; set; }

        /// <summary>
        /// Initializes the non repository members.
        /// </summary>
        private void InitNonRepositoryMembers()
        {
            if (!AutoMapperConfiguration.Initialized)
                AutoMapperConfiguration.Configure();

            var myQuotesListLengt = ConfigurationManager.AppSettings["AGENT_QUOAVDS_NUMBER_OF_RECENT_QUOAVDS_TO_LOAD"];
            DbCacheManager = ServiceLocator.PersistentCache;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PlanningManager"/> class.
        /// </summary>
        /// <param name="entityTypeRepository">The entityType repository.</param>
        public PlanningManager(IRepository<MD_EntityType> entityTypeRepository = null)
        {
            Logger.Instance.LogFunctionEntry("PlanningManager", "PlanningManager()");

            InitNonRepositoryMembers();
            EntityTypeRepository = entityTypeRepository ?? RepositoryFactory.Get<MD_EntityType>();

            var quote = EntityTypeRepository.GetAll();

            Logger.Instance.LogFunctionExit("QuoteManager", "QuoteManager()");
        }

        /// <summary>
        /// Initial Test Method
        /// </summary>
        /// <returns></returns>
        public string StaticInitialTestMethod() => "Initial Test Method_EMRM";
    }
}
