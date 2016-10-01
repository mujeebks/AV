using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVD.DataAccessLayer.Models;

namespace AVD.DataAccessLayer.Repositories
{
    public class EmrmRepository<AVDntity> : Repository<AVDntity> where AVDntity : BaseModel
    {
        internal BaseSystemContext EmrmContext => DbContext as BaseSystemContext;
    }
}
