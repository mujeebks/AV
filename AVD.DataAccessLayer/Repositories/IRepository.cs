using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AVD.DataAccessLayer.Repositories
{
    public interface IRepository<AVDntity> : IDisposable, IDeleteRepository where AVDntity : class
    {
        IEnumerable<AVDntity> Get(Expression<Func<AVDntity, bool>> filter,
            Func<IQueryable<AVDntity>, IOrderedQueryable<AVDntity>> orderBy = null, string includeProperties = "");

        IEnumerable<AVDntity> GetAll();

        AVDntity GetByID(object id);

        void BatchInsertList(List<AVDntity> items, int batchSize = 1000);

        void Insert(AVDntity entity);

        void InsertAndSave(AVDntity entity); //for perf fix

        AVDntity FirstOrDefault(Expression<Func<AVDntity, bool>> predicate = null);

        AVDntity SingleOrDefault(Expression<Func<AVDntity, bool>> predicate);

        AVDntity First(Expression<Func<AVDntity, bool>> predicate = null);

        AVDntity Single(Expression<Func<AVDntity, bool>> predicate);

        [Obsolete]
        AVDntity FirstOrDefaultFunc(Func<AVDntity, bool> predicate = null);
        
        TVal Max<TVal>(Expression<Func<AVDntity, TVal>> func);

        void Delete(object id);

        void Delete(AVDntity entityToDelete);

        void DeleteEntityList(IEnumerable<BaseModel> entities);

        void Update(AVDntity entityToUpdate);

        bool Any(Expression<Func<AVDntity, bool>> predicate);

        [Obsolete]
        bool AnyFunc(Func<AVDntity, bool> predicate);

        int SaveChanges();

        
        /// <summary>
        /// Creates an blank instance of this type that can be navigated.
        /// </summary>
        /// <returns></returns>
        AVDntity Create();


        // Be cautious when using this as it's a leaky abstraction
        IQueryable<AVDntity> GetAsQueryable();
    }
}
