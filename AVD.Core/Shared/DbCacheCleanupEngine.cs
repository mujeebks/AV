using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVD.Common;

namespace AVD.Core.Shared
{
    public class DbCacheCleanupEngine : EngineBase
    {
        public override bool DoWork()
        {
            new DbCacheManager().RemoveExpiredItems();
            return true;
        }
    }
}
