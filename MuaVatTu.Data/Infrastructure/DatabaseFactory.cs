using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MuaVatTu.Data
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly DbContext _dataContext;
        private readonly IConfiguration _configuration;

/*        public DatabaseFactory()
        {
            _dataContext = new MuaVatTuContext.DbContext();
        }*/

        public DbContext GetDbContext()
        {
            return _dataContext;
        }
    }
}
