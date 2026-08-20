using Microsoft.EntityFrameworkCore;
using UniversitySystem3.Common.Exceptions;
using UniversitySystem3.Models;


namespace UniversitySystem3.Repositories;

public class Repository<T> : IRepository<T> where T : class 
{
    protected readonly UniversityDBContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(UniversityDBContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> Query()
    {
        try
        {
            return _dbSet.AsQueryable();
        }
        catch (Exception ex)
        {
            throw new DatabaseReadException($"خطا در خواندن اطلاعات از جدول {typeof(T).Name}.", ex);
        }
    }
    public async Task<T> GetByIdAsync(int id)
    {
        try
        {
            return await _dbSet.FindAsync(id);
        }
        catch (Exception ex)
        {
            throw new DatabaseReadException($"خطا در خواندن رکورد با شناسه {id} از جدول {typeof(T).Name}.", ex);
        }

    }
    public async Task AddAsync(T entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
        }
        catch (Exception ex)
        {
            throw new DatabaseWriteException($"خطا در افزودن رکورد جدید به جدول {typeof(T).Name}.", ex);
        }

    }
    public void Update(T entity)
    {
        try
        {
            _dbSet.Update(entity);
        }
        catch (Exception ex)
        {
            throw new DatabaseWriteException($"خطا در به‌روزرسانی رکورد در جدول {typeof(T).Name}.", ex);
        }


    }
    public void Remove(T entity)
    {

        try
        {
            _dbSet.Remove(entity);
        }
        catch (Exception ex)
        {
            throw new DatabaseWriteException($"خطا در حذف رکورد از جدول {typeof(T).Name}.", ex);
        }
    


}
}
