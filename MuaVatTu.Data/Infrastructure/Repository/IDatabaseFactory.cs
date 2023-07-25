using Microsoft.EntityFrameworkCore;

namespace MuaVatTu.Data
{
    public interface IDatabaseFactory
    {
        DbContext GetDbContext();
    }
}