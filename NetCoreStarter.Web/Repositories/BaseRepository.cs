using Microsoft.EntityFrameworkCore;
using NetCoreStarter.Shared.Filters;
using NetCoreStarter.Utils;
using NetCoreStarter.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreStarter.Web.Repositories
{
    public class BaseRepository<T> : IDisposable where T : class
    {
        public readonly ApplicationDbContext _context;
        protected DbSet<T> DbSet { get; set; }

        public BaseRepository()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>();
            _context = new ApplicationDbContext(options.Options);
            DbSet = _context.Set<T>();
        }

        public T Get(long id) { return DbSet.Find(id); }

        public T Find(Filter<T> filter) { return filter.BuildQuery(DbSet.Select(x => x)).FirstOrDefault(); }

        public List<T> Get() { return DbSet.ToList(); }

        public long Count() { return DbSet.Count(); }

        public long Count(Filter<T> filter) { return filter.BuildQuery(DbSet.Select(x => x)).Count(); }

        public List<T> Get(Filter<T> filter) { return filter.BuildQuery(DbSet.Select(x => x)).ToList(); }

        public virtual IQueryable<T> Query(Filter<T> filter) { return filter.BuildQuery(DbSet.Select(x => x)); }

        public virtual void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public virtual void Insert(T entity)
        {
            DbSet.Add(entity);
            _context.SaveChanges();
        }

        public virtual void BulkInsert(List<T> entries)
        {
            DbSet.AddRange(entries);
            _context.SaveChanges();
        }

        public virtual void Delete(long id)
        {
            var record = DbSet.Find(id);

            var hasLocked = typeof(T).GetProperty(GenericProperties.Locked);
            if (hasLocked != null)
            {
                var islocked = (bool)hasLocked.GetValue(record, null);
                if (islocked) throw new Exception(ExceptionMessage.RecordLocked);
            }

            DbSet.Remove(record);
            _context.SaveChanges();
        }

        public void SaveChanges() { _context.SaveChanges(); }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
