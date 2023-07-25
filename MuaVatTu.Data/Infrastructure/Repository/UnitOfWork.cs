using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace MuaVatTu.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDatabaseFactory _databaseFactory;
        private DbContext _dataContext;
        private bool _disposed;
        public List<IRepositoryBase> ListRepository = new List<IRepositoryBase>();

        public UnitOfWork(IDatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
            _dataContext = _databaseFactory.GetDbContext();
        }

        public UnitOfWork(DatabaseFactory databaseFactory)
        {
        }

        public DbContext DataContext => _dataContext ?? (_dataContext = _databaseFactory.GetDbContext());

        public IRepository<T> GetRepository<T>() where T : class
        {
            var repository = new Repository<T>(_dataContext);
            ListRepository.Add(repository);
            return repository;
        }

        public int Save()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public bool CheckConnection()
        {
            throw new NotImplementedException();
        }

        public List<dynamic> SelectByCommand(string rawSQL, List<SqlParameter> listParameter)
        {
            throw new NotImplementedException();
        }

        public DbContext GetDbContext()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                {
                    _dataContext.Dispose();
                    _disposed = true;
                }

            _disposed = false;
        }
    }
}