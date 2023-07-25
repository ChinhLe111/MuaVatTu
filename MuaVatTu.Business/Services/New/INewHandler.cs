using MuaVatTu.Common;
using System;
using System.Threading.Tasks;

namespace MuaVatTu.Business
{
    public interface INewHandler
    {
        Task<Response> Get(NewQueryModel filter);
        Task<Response> GetById(Guid id);
        Task<Response> Create (AddNewModel filter);
        Task<Response> Update(Guid id, UpdateNewModel filter);
        Task<Response> Delete(Guid id);
        Task<Response> CreateData();
    }
}
