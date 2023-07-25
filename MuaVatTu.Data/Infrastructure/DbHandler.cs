using AutoMapper;
using MuaVatTu.Common;
using Microsoft.EntityFrameworkCore;
using MuaVatTu.Common.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace MuaVatTu.Data
{
    public class DbHandler<TDbModel, TResultModel, TQueryModel>
        where TDbModel : class
        where TResultModel : class
        where TQueryModel : PaginationRequest
    {
        public static DbHandler<TDbModel, TResultModel, TQueryModel> Instance { get; } =
            new DbHandler<TDbModel, TResultModel, TQueryModel>();

        public async Task<bool> CheckPropValidate(TDbModel request, bool isUpdate, Guid? appId = null,
            Guid? actorId = null, params string[] propValidate)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    if (propValidate.Length == 0)
                    {
                        return false;
                    }
                    var checkQuery = unitOfWork.GetRepository<TDbModel>().GetAll();
                    if (isUpdate)
                    {
                        var compare = request.GetPropValue("Id");
                        checkQuery = checkQuery.Where("Id", compare, ExtensionMethods.ExpressionOption.NotEqual);
                    }

                    foreach (var prop in propValidate)
                    {
                        var compare = request.GetPropValue(prop);
                        checkQuery = checkQuery.Where(prop, compare, ExtensionMethods.ExpressionOption.Equal);
                    }

                    var result = await checkQuery.AnyAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return false;
            }
        }

        #region CRUD

        public async Task<Response> FindAsync(Guid id, Guid? appId = null, Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var checkExiest = await unitOfWork.GetRepository<TDbModel>().FindAsync(id);
                    if (checkExiest == null)
                    {
                        Log.Error($"{id} not found");
                        return new ResponseError(HttpStatusCode.BadRequest, "Id không tồn tại");
                    }

                    return new ResponseObject<TResultModel>(
                        AutoMapperUtils.AutoMap<TDbModel, TResultModel>(checkExiest));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> CreateAsync(TDbModel request, params string[] propValidate)
        {
            return await CreateAsync(request, null, null, propValidate);
        }

        public async Task<Response> CreateAsync(TDbModel request, Guid? appId = null, Guid? actorId = null,
            params string[] propValidate)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    if (propValidate != null && propValidate.Length > 0)
                    {
                        var check = await CheckPropValidate(request, false, appId, actorId, propValidate);
                        if (check)
                        {
                            Log.Error("The sql statement is not executed!");
                            return new ResponseError(HttpStatusCode.BadRequest,
                                "Trùng thông tin. Vui lòng kiểm tra thuộc tính: " + string.Join(", ", propValidate));
                        }
                    }

                    unitOfWork.GetRepository<TDbModel>().Add(request);
                    if (await unitOfWork.SaveAsync() > 0)
                        return new ResponseObject<TResultModel>(
                            AutoMapperUtils.AutoMap<TDbModel, TResultModel>(request));

                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> UpdateAsync(Guid id, TDbModel request, params string[] propValidate)
        {
            return await UpdateAsync(id, request, null, null, propValidate);
        }

        public async Task<Response> UpdateAsync(Guid id, TDbModel request, Guid? appId = null, Guid? actorId = null,
            params string[] propValidate)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    if (propValidate != null && propValidate.Length > 0)
                    {
                        var check = await CheckPropValidate(request, true, appId, actorId, propValidate);
                        if (check)
                        {
                            Log.Error("The sql statement is not executed!");
                            return new ResponseError(HttpStatusCode.BadRequest,
                                "Trùng thông tin. Vui lòng kiểm tra thuộc tính: " + string.Join(", ", propValidate));
                        }
                    }

                    var checkExiest = await unitOfWork.GetRepository<TDbModel>().AnyAsync(id);

                    if (!checkExiest)
                    {
                        Log.Error($"{id} not found");
                        return new ResponseError(HttpStatusCode.BadRequest, "Id không tồn tại");
                    }

                    unitOfWork.GetRepository<TDbModel>().Update(request);
                    if (await unitOfWork.SaveAsync() > 0)
                        return new ResponseUpdate(id);

                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> UpdateAsync(Expression<Func<TDbModel, bool>> predicate, TDbModel request,
            Guid? appId = null, Guid? actorId = null, params string[] propValidate)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var checkExiest = await unitOfWork.GetRepository<TDbModel>().AnyAsync(predicate);
                    if (!checkExiest)
                    {
                        return new ResponseError(HttpStatusCode.NotFound, "Không tìm được dữ liệu");
                    }

                    if (propValidate != null)
                    {
                        var check = await CheckPropValidate(request, true, appId, actorId, propValidate);
                        if (check)
                        {
                            Log.Error("The sql statement is not executed!");
                            return new ResponseError(HttpStatusCode.BadRequest,
                                "Trùng thông tin. Vui lòng kiểm tra thuộc tính: " + string.Join(", ", propValidate));
                        }
                    }

                    unitOfWork.GetRepository<TDbModel>().Update(request);
                    if (await unitOfWork.SaveAsync() > 0)
                        return new ResponseUpdate(Guid.Empty);

                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> DeleteAsync(Guid id, Guid? appId = null, Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var checkExiest = await unitOfWork.GetRepository<TDbModel>().FindAsync(id);
                    if (checkExiest == null)
                    {
                        Log.Error($"{id} not found");
                        return new ResponseError(HttpStatusCode.BadRequest, "Id không tồn tại");
                    }

                    var name = "";
                    try
                    {
                        name = checkExiest.GetPropValue("Name").ToString();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    unitOfWork.GetRepository<TDbModel>().Delete(checkExiest);
                    if (await unitOfWork.SaveAsync() > 0)
                        return new ResponseDelete(id, name);

                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> DeleteRangeAsync(List<Guid> listId, string idName, Guid? appId = null,
            Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var result = new List<ResponseDelete>();
                    var listCurrent = await unitOfWork.GetRepository<TDbModel>().GetAll().WhereContains(idName, listId)
                        .ToListAsync();
                    if (listCurrent.Count == 0)
                    {
                        Log.Error($"{listId} not found");
                        return new ResponseError(HttpStatusCode.BadRequest, "danh sách Id không tồn tại");
                    }

                    foreach (var id in listId)
                    {
                        var current = listCurrent.AsQueryable()
                            .Where(idName, id, ExtensionMethods.ExpressionOption.Equal).FirstOrDefault();
                        if (current != null)
                        {
                            var name = "";
                            try
                            {
                                name = current.GetPropValue("Name").ToString();
                            }
                            catch (Exception)
                            {
                                // ignored
                            }

                            result.Add(new ResponseDelete(id, name));
                        }
                        else
                        {
                            result.Add(new ResponseDelete(HttpStatusCode.BadRequest, "Id không tồn tại", id, ""));
                        }
                    }

                    unitOfWork.GetRepository<TDbModel>().DeleteRange(listCurrent);
                    if (await unitOfWork.SaveAsync() > 0)
                        return new ResponseDeleteMulti(result);

                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> GetPageAsync(Expression<Func<TDbModel, bool>> predicate, TQueryModel query,
            Guid? appId = null, Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var result = await unitOfWork.GetRepository<TDbModel>().GetPageAsync(predicate, query);
                    if (result != null)
                        return new ResponsePagination<TResultModel>(new Pagination<TResultModel> {

                            Content = AutoMapperUtils.AutoMap<TDbModel, TResultModel>(result.Content),
                            NumberOfElements = result.NumberOfElements,
                            Page = result.Page,
                            Size = result.Size,
                            TotalElements = result.TotalElements,
                            TotalPages = result.TotalPages
                        });

                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> GetAllAsync(Expression<Func<TDbModel, bool>> predicate, string sort = "", Guid? appId = null,
            Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var result = await unitOfWork.GetRepository<TDbModel>().GetListAsync(predicate, sort);
                    if (result != null)

                        return new ResponseObject<List<TResultModel>>(
                            AutoMapperUtils.AutoMap<TDbModel, TResultModel>(result));

                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<ResponseObject<int>> CountAllAsync(Expression<Func<TDbModel, bool>> predicate)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var count = await unitOfWork.GetRepository<TDbModel>().CountAsync(predicate);
                    return new ResponseObject<int>(count);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseObject<int>(0);
            }
        }

        public Response GetAll(Guid? appId = null, Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var result = unitOfWork.GetRepository<TDbModel>().GetAll().AsNoTracking();
                    if (result != null)
                        return new ResponseObject<List<TResultModel>>(
                            AutoMapperUtils.AutoMap<TDbModel, TResultModel>(result.ToList()));

                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        #endregion CRUD


        #region custom

        public async Task<Response> GetPageAsync(Expression<Func<TDbModel, bool>> predicate, TQueryModel query, IMapper mapper,
            Guid? appId = null, Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var result = await unitOfWork.GetRepository<TDbModel>().GetPageAsync(predicate, query);
                    if (result != null)
                        return new ResponsePagination<TResultModel>(
                            mapper.Map<Pagination<TDbModel>, Pagination<TResultModel>>(result));

                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> GetPageAsync(IQueryable<TDbModel> iqueryable, TQueryModel query,
            IMapper _mapper)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var result = await unitOfWork.GetRepository<TDbModel>().GetPageAsync(iqueryable, query);

                    if (result != null)
                        if (_mapper != null)
                        {
                            return new ResponsePagination<TResultModel>(
                           _mapper.Map<Pagination<TDbModel>, Pagination<TResultModel>>(result));
                        }
                        else
                        {
                            return new ResponsePagination<TResultModel>(
                           AutoMapperUtils.AutoMap<Pagination<TDbModel>, Pagination<TResultModel>>(result));
                        }
                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> GetAllAsyncCustom(IQueryable<TDbModel> iqueryable, IMapper _mapper = null, string sort = "", Guid? appId = null,
     Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var result = await unitOfWork.GetRepository<TDbModel>().GetListAsyncCustom(iqueryable, sort);
                    if (result != null)
                        if (_mapper != null)
                        {
                            return new ResponseObject<List<TResultModel>>(
                           _mapper.Map<List<TDbModel>, List<TResultModel>>(result));
                        }
                        else
                        {
                            return new ResponseObject<List<TResultModel>>(
                            AutoMapperUtils.AutoMap<TDbModel, TResultModel>(result));
                        }

                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> GetAllAsyncCustom(IQueryable<TResultModel> iqueryable, string sort = "", Guid? appId = null,
         Guid? actorId = null)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var result = await iqueryable.ApplySorting(sort).ToListAsync();
                    if (result != null)
                        return new ResponseObject<List<TResultModel>>(result);

                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        public async Task<Response> GetPageAsyncCustom(IQueryable<TResultModel> iqueryable, TQueryModel query)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var result = new Pagination<TResultModel>();
                    iqueryable = iqueryable.AsNoTracking();
                    //var dataSet = _dataContext.Set<T>().Include(_dataContext.GetIncludePaths(typeof(T)));
                    query.Page = query.Page ?? 1;
                    if (query.Sort != null && query.Size.HasValue)
                    {
                        iqueryable = iqueryable.ApplySorting(query.Sort);
                        var totals = await iqueryable.CountAsync();
                        var totalsPages = (int)Math.Ceiling(totals / (float)query.Size.Value);
                        var excludedRows = (query.Page.Value - 1) * query.Size.Value;
                        iqueryable = iqueryable.Skip(excludedRows).Take(query.Size.Value);
                        var items = iqueryable.ToList();

                        result = new Pagination<TResultModel>
                        {
                            Page = query.Page.Value,
                            Content = items,
                            NumberOfElements = items.Count(),
                            Size = query.Size.Value,
                            TotalPages = totalsPages,
                            TotalElements = totals
                        };
                    }

                    if (!query.Size.HasValue)
                    {
                        var totals = await iqueryable.CountAsync();
                        var items = await iqueryable.ToListAsync();
                        result = new Pagination<TResultModel>
                        {
                            Page = 1,
                            Content = items,
                            NumberOfElements = totals,
                            Size = totals,
                            TotalPages = 1,
                            TotalElements = totals
                        };
                    }

                    if (result != null)
                        return new ResponsePagination<TResultModel>(result);
                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                return new ResponseError(HttpStatusCode.InternalServerError, "Có lỗi trong quá trình xử lý: " + ex.Message);
            }
        }

        #endregion custom
    }
}