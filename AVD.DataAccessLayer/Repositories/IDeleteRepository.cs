using System.Collections.Generic;

namespace AVD.DataAccessLayer.Repositories
{
    public interface IDeleteRepository
    {
        void DeleteEntityList(IEnumerable<BaseModel> entities);
    }
}
