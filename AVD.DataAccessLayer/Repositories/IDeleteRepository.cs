using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.DataAccessLayer
{
    public interface IDeleteRepository
    {
        void DeleteEntityList(IEnumerable<BaseModel> entities);
    }
}
