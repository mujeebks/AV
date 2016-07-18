using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace AVD.DataAccessLayer
{
    public interface IRepository<TEntity> : IDisposable, IDeleteRepository where TEntity : class
    {
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");

        IEnumerable<TEntity> GetAll();

        TEntity GetByID(object id);

        void BatchInsertList(List<TEntity> items, int batchSize = 1000);

        void Insert(TEntity entity);

        void InsertAndSave(TEntity entity); //for perf fix

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        TEntity First(Expression<Func<TEntity, bool>> predicate = null);

        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        [Obsolete]
        TEntity FirstOrDefaultFunc(Func<TEntity, bool> predicate = null);
        
        TVal Max<TVal>(Expression<Func<TEntity, TVal>> func);

        void Delete(object id);

        void Delete(TEntity entityToDelete);

        void DeleteEntityList(IEnumerable<BaseModel> entities);

        void Update(TEntity entityToUpdate);

        bool Any(Expression<Func<TEntity, bool>> predicate);

        [Obsolete]
        bool AnyFunc(Func<TEntity, bool> predicate);

        int SaveChanges();

        
        /// <summary>
        /// Creates an blank instance of this type that can be navigated.
        /// </summary>
        /// <returns></returns>
        TEntity Create();


        // Be cautious when using this as it's a leaky abstraction
        IQueryable<TEntity> GetAsQueryable();
    }
}
