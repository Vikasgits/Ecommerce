using E_commerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace E_commerce.Repository

{
    public interface IEntityRepository<T> where T : class
    {
        public IQueryable<T> Get();
        public T Get(Guid id);
        public bool Add(T entity);
        public T Update(T entity);
        public bool Delete(T entity);

        public IDbContextTransaction BeginTransaction();
        public EcommerceContext GetContext();
    }
}
