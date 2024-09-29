
using E_commerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace E_commerce.Repository
{
    public class EntityRepository<T> : IEntityRepository<T> where T : class
    {
        private  EcommerceContext _context;
        private readonly DbSet<T> _table;
        private IDbContextTransaction _transaction;
        public EntityRepository(EcommerceContext context) {
            _context = context;
            _table = _context.Set<T>();
        }
        public IQueryable<T> Get()
        {
            try
            {
                return _table.AsQueryable();
            }
            catch (Exception ex) {
                throw ex;

            }
           }
        public T Get(Guid id)
        {
            try
            {
                var entity = _table.Find(id);
                if (entity == null) { return entity; }
                _context.Entry(entity).State = EntityState.Detached;
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            
            }
        public bool Add(T entity)
        {
            try
            {
                _table.Add(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool Delete(T entity)
        {
            try
            {
                var isActiveProperty = entity.GetType().GetProperty("IsActive");
                var isActiveOrder = entity.GetType().GetProperty("ActiveOrder");
                var isAvailableProduct = entity.GetType().GetProperty("IsAvailable");
                if (isActiveProperty != null)
                {
                    isActiveProperty.SetValue(entity, false);
                    _context.Entry(entity).State = EntityState.Modified;
                }
                if (isAvailableProduct != null)
                {
                    isAvailableProduct.SetValue(entity, false);
                    _context.Entry(entity).State = EntityState.Modified;
                }
                else if (isActiveOrder != null)
                {
                    isActiveOrder.SetValue(entity, false);
                    _context.Entry(entity).State = EntityState.Modified;
                }
                else _table.Remove(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T Update(T entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                _table.Update(entity);
                _context.SaveChanges();
                return entity;
            }
            catch (Exception ex) {
                throw ex;
            }

        }


        public EcommerceContext GetContext()
        {
            return _context;
        }

        public IDbContextTransaction BeginTransaction()
        {
            _transaction=_context.Database.BeginTransaction();
            return _transaction;
        }
    }
}

   
