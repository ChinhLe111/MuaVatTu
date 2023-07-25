using MuaVatTu.Common;
using MuaVatTu.Data;
using System;
using System.Threading.Tasks;

namespace MuaVatTu.Business
{
    public interface IBoPhanHandler
    {
        Task<Response> Get(BoPhanQueryModel filter);
        Object Query(string s, string sort, int? queryPage);
        Task<Response> GetById(Guid id);
        Task<Response> GetBoPhan();
        Task<Response> Create(BoPhanAddModel filter);
        Task<Response> Update(Guid id, BoPhanUpdateModel filter);
        Task<Response> Delete(Guid id);
        Task<Response> CreateData();
    }
}
