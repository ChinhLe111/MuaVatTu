using MuaVatTu.Common;
using System;
using System.Threading.Tasks;

namespace MuaVatTu.Business
{
    public interface INhanVienHandler
    {
        Task<Response> Get(NhanVienQueryModel filter);
        Task<Response> GetNhanVienForMonth();
        Task<Response> GetNhanVienFromBoPhan();
        Task<Response> GetById(Guid id);
        Task<Response> Create(Guid boPhanId, AddNhanVienModel filter);
        Task<Response> Update(Guid id, UpdateNhanVienModel filter);
        Task<Response> Delete(Guid id);
        Task<Response> CreateData();
        Task<Response> GetMaxNhanVienForMonth();
    }
}
