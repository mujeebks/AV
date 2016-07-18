using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVD.DataAccessLayer.Models;

namespace AVD.DataAccessLayer.Repositories
{
    public class EmrmRepository<TEntity> : Repository<TEntity> where TEntity : BaseModel
    {
        internal BaseSystemContext EmrmContext
        {
            get { return DbContext as BaseSystemContext; }
        }
    }
}
