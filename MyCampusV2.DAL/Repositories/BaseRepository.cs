using Microsoft.EntityFrameworkCore;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyCampusV2.DAL.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected MyCampusCardContext _context;

        public BaseRepository(MyCampusCardContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public virtual async Task<ICollection<T>> GetAllAsyn()
        {

            return await _context.Set<T>().ToListAsync();
        }

        public virtual T Get(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public virtual async Task<T> GetAsync(object id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual T Add(T t)
        {

            _context.Set<T>().Add(t);
            _context.SaveChanges();
            return t;
        }

        public virtual async Task<T> AddAsyn(T t)
        {
            _context.Set<T>().Add(t);
            await _context.SaveChangesAsync();
            return t;
        }

        public virtual async Task<T> AddAsyncWithBase(T t)
        {
            try
            {
                t.GetType().GetProperty("Date_Time_Added").SetValue(t, DateTime.UtcNow.ToLocalTime(), null);
                t.GetType().GetProperty("Last_Updated").SetValue(t, DateTime.UtcNow.ToLocalTime(), null);
                t.GetType().GetProperty("IsActive").SetValue(t, true, null);
                t.GetType().GetProperty("ToDisplay").SetValue(t, true, null);
                _context.Set<T>().Add(t);
                await _context.SaveChangesAsync();
                return t;
            } catch (Exception err)
            {
                return null;
            }
        }

        public virtual T Find(Expression<Func<T, bool>> match)
        {
            return _context.Set<T>().SingleOrDefault(match);
        }

        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(match);
        }

        public ICollection<T> FindAll(Expression<Func<T, bool>> match)
        {
            return _context.Set<T>().Where(match).ToList();
        }

        public async Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().Where(match).ToListAsync();
        }

        public virtual void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public virtual async Task<int> DeleteAsyn(T entity)
        {
            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public virtual T Update(T t, object key)
        {
            if (t == null)
                return null;
            T exist = _context.Set<T>().Find(key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(t);
                _context.SaveChanges();
            }
            return exist;
        }

        public virtual async Task<T> UpdateAsyn(T t, object key)
        {
            if (t == null)
                return null;
            T exist = await _context.Set<T>().FindAsync(key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(t);
                await _context.SaveChangesAsync();
            }
            return exist;
        }

        public virtual async Task<T> RetrieveAsync(T t, object key)
        {
            try
            {
                T exist = await _context.Set<T>().FindAsync(key);

                if (exist != null)
                {
                    //t = exist;

                    t.GetType().GetProperty("Date_Time_Added").SetValue(t, exist.GetType().GetProperty("Date_Time_Added").GetValue(exist, null), null);
                    t.GetType().GetProperty("Last_Updated").SetValue(t, DateTime.UtcNow.ToLocalTime(), null);
                    t.GetType().GetProperty("IsActive").SetValue(t, true, null);
                    t.GetType().GetProperty("ToDisplay").SetValue(t, true, null);
                    t.GetType().GetProperty("Added_By").SetValue(t, exist.GetType().GetProperty("Added_By").GetValue(exist, null), null);
                    _context.Entry(exist).CurrentValues.SetValues(t);
                    await _context.SaveChangesAsync();

                    return exist;
                }
                else
                {
                    return null;
                }
            } catch (Exception err)
            {
                return null;
            }
        }

        public virtual async Task<T> DeleteAsyncTemporary(T t, object key)
        {
            try
            {
                T exist = await _context.Set<T>().FindAsync(key);

                if (exist != null)
                {
                    //t = exist;

                    t.GetType().GetProperty("Date_Time_Added").SetValue(t, exist.GetType().GetProperty("Date_Time_Added").GetValue(exist, null), null);
                    t.GetType().GetProperty("Last_Updated").SetValue(t, DateTime.UtcNow.ToLocalTime(), null);

                    if (t.GetType().GetProperty("IsActive").GetValue(exist, null).Equals(true))
                        t.GetType().GetProperty("IsActive").SetValue(t, false, null);
                    else
                        t.GetType().GetProperty("IsActive").SetValue(t, true, null);

                    t.GetType().GetProperty("Added_By").SetValue(t, exist.GetType().GetProperty("Added_By").GetValue(exist, null), null);
                    _context.Entry(exist).CurrentValues.SetValues(t);
                    await _context.SaveChangesAsync();

                    return exist;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception err)
            {
                return null;
            }
        }

        public virtual async Task<T> DeleteAsyncPermanent(T t, object key)
        {
            T exist = await _context.Set<T>().FindAsync(key);

            if (exist != null)
            {
                t.GetType().GetProperty("Date_Time_Added").SetValue(t, exist.GetType().GetProperty("Date_Time_Added").GetValue(exist, null), null);
                t.GetType().GetProperty("Last_Updated").SetValue(t, DateTime.UtcNow.ToLocalTime(), null);
                t.GetType().GetProperty("IsActive").SetValue(t, false, null);
                t.GetType().GetProperty("ToDisplay").SetValue(t, false, null);
                t.GetType().GetProperty("Added_By").SetValue(t, exist.GetType().GetProperty("Added_By").GetValue(exist, null), null);
                _context.Entry(exist).CurrentValues.SetValues(t);
                await _context.SaveChangesAsync();

                return exist;
            }
            else
            {
                return null;
            }
        }

        public virtual async Task<T> UpdateAsyncWithBase(T t, object key)
        {
            try
            {
                if (t == null)
                    return null;
                T exist = await _context.Set<T>().FindAsync(key);
                if (exist != null)
                {
                    t.GetType().GetProperty("Date_Time_Added").SetValue(t, exist.GetType().GetProperty("Date_Time_Added").GetValue(exist, null), null);
                    t.GetType().GetProperty("Last_Updated").SetValue(t, DateTime.UtcNow.ToLocalTime(), null);
                    //t.GetType().GetProperty("IsActive").SetValue(t, exist.GetType().GetProperty("IsActive").GetValue(exist, null), null);
                    t.GetType().GetProperty("ToDisplay").SetValue(t, exist.GetType().GetProperty("ToDisplay").GetValue(exist, null), null);
                    t.GetType().GetProperty("Added_By").SetValue(t, exist.GetType().GetProperty("Added_By").GetValue(exist, null), null);
                    _context.Entry(exist).CurrentValues.SetValues(t);
                    await _context.SaveChangesAsync();
                }
                return exist;
            } catch (Exception err)
            {
                return null;
            }
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public virtual async Task Reload(T t)
        {
            await _context.Entry(t).ReloadAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        public virtual void Save()
        {
            _context.SaveChanges();
        }

        public async virtual Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            return query;
        }

        public virtual async Task<ICollection<T>> FindByAsyn(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {

            IQueryable<T> queryable = GetAll();
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {

                queryable = queryable.Include<T, object>(includeProperty);
            }

            return queryable;
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
