using MuaVatTu.Common.Helpers;
using MuaVatTu.Data;
using MuaVatTu.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MuaVatTu.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbset;
        private DbContext _dataContext;

        public List<Task> ListEsTask = new List<Task>();

        public Repository(DbContext dataContext)
        {
            _dataContext = dataContext;
            _dbset = _dataContext.Set<T>();
        }

        //protected IDatabaseFactory DatabaseFactory { get; }

        //protected DbContext DataContext => _dataContext ?? (_dataContext = DatabaseFactory.GetDbContext());

        //public List<Task> GetAllTask()
        //{
        //    return ListEsTask;
        //}

        public IQueryable<T> SqlQuery(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public T Find(params object[] id)
        {
            throw new NotImplementedException();
        }

        public T Find(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindAsync(params object[] id)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public bool Any(params object[] id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(params object[] id)
        {
            throw new NotImplementedException();
        }

        public virtual int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public long LongCount()
        {
            throw new NotImplementedException();
        }

        public long LongCount(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<long> LongCountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<long> LongCountAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            //foreach (var entity in entities)
            //{
            //    _dbset.Add(entity);
            //}
            throw new NotImplementedException();
        }

        public void AddRange(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public void DeleteRange(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public void DeleteRange(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate, string sort = "")
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetInclude(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, string sort = "")
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetListAsync(IQueryable<T> iqueryable, string sort = "")
        {
            throw new NotImplementedException();
        }

        public DbSet<T> DbSet()
        {
            throw new NotImplementedException();
        }

        #region GetPage

        public Pagination<T> GetPage(PaginationRequest query)
        {
            var dataSet = _dbset.AsQueryable().AsNoTracking();
            query.Page = query.Page ?? 1;
            if (query.Sort != null && query.Size.HasValue)
            {

                var totals = dataSet.Count();
                var totalsPages = (int)Math.Ceiling(totals / (float)query.Size.Value);
                var excludedRows = (query.Page.Value - 1) * query.Size.Value;
                var items = dataSet.Skip(excludedRows).Take(query.Size.Value).ToList();
                items.RemoveAt(items.Count - 1);
                return new Pagination<T>
                {
                    Page = query.Page.Value,
                    Content = items,
                    NumberOfElements = items.Count,
                    Size = query.Size.Value,
                    TotalPages = totalsPages,
                    TotalElements = totals
                };
            }

            if (!query.Size.HasValue)
            {
                var totals = dataSet.Count();
                var items = dataSet.ToList();
                return new Pagination<T>
                {
                    Page = 1,
                    Content = items,
                    NumberOfElements = totals,
                    Size = totals,
                    TotalPages = 1,
                    TotalElements = totals
                };
            }

            return null;
        }

        public Pagination<T> GetPage()
        {
            return GetPage(new PaginationRequest { Size = null });
        }

        public Pagination<T> GetPage(Expression<Func<T, bool>> predicate, PaginationRequest query)
        {
            var dataSet = _dbset.AsQueryable().AsNoTracking();
            query.Page = query.Page ?? 1;
            if (query.Sort != null && query.Size.HasValue)
            {

                var totals = dataSet.Count();
                var totalsPages = (int)Math.Ceiling(totals / (float)query.Size.Value);
                var excludedRows = (query.Page.Value - 1) * query.Size.Value;
                var items = dataSet.Skip(excludedRows).Take(query.Size.Value).ToList();
                items.RemoveAt(items.Count - 1);
                return new Pagination<T>
                {
                    Page = query.Page.Value,
                    Content = items,
                    NumberOfElements = items.Count,
                    Size = query.Size.Value,
                    TotalPages = totalsPages,
                    TotalElements = totals
                };
            }

            if (!query.Size.HasValue)
            {
                var totals = dataSet.Count();
                var items = dataSet.ToList();
                return new Pagination<T>
                {
                    Page = 1,
                    Content = items,
                    NumberOfElements = totals,
                    Size = totals,
                    TotalPages = 1,
                    TotalElements = totals
                };
            }

            return null;
        }


        public async Task<Pagination<T>> GetPageAsync(PaginationRequest query)
        {
            throw new NotImplementedException();
        }

        public async Task<Pagination<T>> GetPageAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Pagination<T>> GetPageAsync(Expression<Func<T, bool>> predicate,
            PaginationRequest query)
        {
            throw new NotImplementedException();
        }

        public async Task<Pagination<T>> GetPageAsync(IQueryable<T> iqueryable, PaginationRequest query)
        {
            throw new NotImplementedException();
        }

        #endregion GetPage

        public List<Task> GetAllTask()
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetListAsyncCustom(IQueryable<T> iqueryable, string sort = "")
        {
            throw new NotImplementedException();
        }
    }
}