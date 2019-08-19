using Microsoft.EntityFrameworkCore;
using NetCoreStarter.Shared.Filters;
using NetCoreStarter.Utils;
using NetCoreStarter.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCoreStarter.Data.Repositories
{
    public class BaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext DbContext;
        protected Microsoft.EntityFrameworkCore.DbSet<T> DbSet { get; set; }

        public BaseRepository()
        {
            var options = new DbContextOptions<ApplicationDbContext>();
            DbContext = new ApplicationDbContext(options);
            DbSet = DbContext.Set<T>();
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
            DbContext.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            DbContext.SaveChanges();
        }

        public virtual void Insert(T entity)
        {
            DbSet.Add(entity);
            DbContext.SaveChanges();
        }

        public virtual void BulkInsert(List<T> entries)
        {
            DbSet.AddRange(entries);
            DbContext.SaveChanges();
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
            DbContext.SaveChanges();
        }

        public void SaveChanges() { DbContext.SaveChanges(); }
    }
}
