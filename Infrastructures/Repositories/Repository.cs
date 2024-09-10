using Eduhunt.Data;
using Eduhunt.Models.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Eduhunt.Infrastructures.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IHasId, IHasSoftDelete
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public Repository(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual IQueryable<T> GetAll()
        {
            return _context.Set<T>()
                .AsNoTracking();
        }

        public virtual async Task<T?> GetByIdAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Unable to process, id is null");
            }

            var entity = await _context.Set<T>()
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity;
        }

        public virtual async Task AddAsync(T? entity)
        {
            if (entity != null)
            {
                _context.Set<T>().Add(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Unable to process, entity is null");
            }
        }

        public virtual async Task UpdateAsync(T? entity)
        {
            if (entity != null)
            {
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Unable to process, entity is null");
            }
        }

        public virtual async Task DeleteAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Unable to process, id is null");
            }

            var entity = await _context.Set<T>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity != null)
            {

                if (entity is IHasSoftDelete softDeleteEntity)
                {
                    softDeleteEntity.IsNotDeleted = false;
                    _context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    _context.Set<T>().Remove(entity);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
