using MuaVatTu.Common;
using System;
using System.Threading.Tasks;

namespace MuaVatTu.Business
{
    public interface IDangKyHandler 
    {
        Task<Response> Get(DangKyQueryModel filter);
        Task<Response> GetDangKyForMonth();
        Task<Response> GetById(Guid id);
        Task<Response> Create(DangKyCreateModel filter);
        Task<Response> Update(Guid id, DangKyUpdateModel filter);
        Task<Response> Delete(Guid id);
        Task<Response> DeleteProduct(Guid dangKyId ,Guid id);
        Task<Response> CreateProduct(Guid dangKyId, MatHangDto filter);
        Task<Response> CreateData();
    }
}
