using MuaVatTu.Common;
using System;
using System.Threading.Tasks;

namespace MuaVatTu.Business
{
    public interface IMatHangHandler
    {
        Task<Response> Get(MatHangQueryModel filter);
        Task<Response> GetMaxCount();
        Task<Response> GetMonthMaxCount();
        Task<Response> GetByDangKyId(Guid dangKyId, Guid id);
        Task<Response> GetById(Guid id);
        Task<Response> Create(Guid dangKyId, AddMatHangModel filter);
        Task<Response> Update(Guid id, UpdateMatHangModel filter);
        Task<Response> Delete(Guid dangKyId);
        Task<Response> CreateData(Guid dangKyId);
    }
}
