using AutoMapper;
using Bogus;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using MuaVatTu.Common;
using MuaVatTu.Common.Helpers;
using MuaVatTu.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Code = System.Net.HttpStatusCode;

namespace MuaVatTu.Business
{
    public class NhanVienHandler : INhanVienHandler
    {
        private readonly MuaVatTuContext _context;
        private readonly IMapper _mapper;
        public NhanVienHandler(MuaVatTuContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #region CRUD
        public async Task<Response> Get(NhanVienQueryModel filter)
        {
            var NhanVienFromRepo = await _context.NhanViens.FirstOrDefaultAsync();
            if (NhanVienFromRepo == null)
                return new Response(Code.OK, "Không tồn tại");
            try
            {
                var predicate = BuildQuery(filter);
                var result = await _context.NhanViens.GetPageAsync(filter);
                var data = _mapper.Map<Pagination<NhanVien>, Pagination<NhanVienDto>>(result);
                return new ResponsePagination<NhanVienDto>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }

        public async Task<Response> Create(Guid boPhanId, AddNhanVienModel filter)
        {
            var NhanVienFromRepo = await _context.NhanViens.Where(x => x.BoPhanId == boPhanId).FirstOrDefaultAsync();
            if (NhanVienFromRepo == null)
                return new Response(Code.OK, "Không tồn tại");

            try
            {
                var nhanVien = _mapper.Map<NhanVien>(filter);
                nhanVien.BoPhanId = boPhanId;
                _context.Add(nhanVien);

                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    var data = _mapper.Map<NhanVienDto>(nhanVien);
                    return new ResponseObject<NhanVienDto>(data, "Thêm mới thành công");


                }

                return new ResponseError(Code.NotFound, "Thêm mới thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }

        public async Task<Response> Delete(Guid id)
        {
            var NhanVienFromRepo = await _context.NhanViens.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (NhanVienFromRepo == null)
                return new Response(Code.OK, "Không tồn tại");

            _context.NhanViens.Remove(NhanVienFromRepo);
            var status = await _context.SaveChangesAsync();
            return new Response(Code.OK, "Xóa thành công");
        }
        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var entity = await _context.NhanViens.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (entity == null)
                    return new ResponseError(Code.NotFound, "Không tìm thấy nhân viên");

                var data = _mapper.Map<NhanVien, NhanVienDto>(entity);
                return new ResponseObject<NhanVienDto>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }

        }
        public async Task<Response> Update(Guid id, UpdateNhanVienModel model)
        {
            try
            {

                var entityModel = await _context.NhanViens.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (entityModel == null)
                    return new ResponseError(Code.NotFound, "Không tìm thấy bản ghi");

                _mapper.Map(model, entityModel);

                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    var data = _mapper.Map<NhanVienDto>(entityModel);
                    return new ResponseObject<NhanVienDto>(data, "Cập nhật thành công");

                }

                return new ResponseError(Code.NotFound, "Cập nhật thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }

        }
        private Expression<Func<NhanVien, bool>> BuildQuery(NhanVienQueryModel query)
        {
            var predicate = PredicateBuilder.New<NhanVien>(true);

            if (query.Id.HasValue && query.Id.Value != Guid.Empty)
            {
                predicate = predicate.And(s => s.Id == query.Id);
            }

            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.Name.Contains(query.FullTextSearch)
                || s.Name.Contains(query.FullTextSearch.ToLower()));
            return predicate;
        }
        public async Task<Response> CreateData()
        {
            var getBoPhan = await _context.BoPhans.Select(e => e.Id).ToListAsync();
            var dangKyFaker = new Faker<NhanVien>()
              .RuleFor(x => x.Name, x => x.Name.FirstName())
              .RuleFor(x => x.Date, x => x.Date.Recent(31))
              .RuleFor(x => x.BoPhanId, x => x.PickRandom(getBoPhan));
            _context.AddRange(dangKyFaker.Generate(1000));
            _context.SaveChanges();

            return null;
        }

        /// <summary>
        /// Thống kê số lượng nhân viên mới theo tháng
        /// </summary>
        /// <returns></returns>
        public async Task<Response> GetNhanVienForMonth()
        {
            List<ListMonth> listMonth = new List<ListMonth>();
            var getNhanVien = await _context.NhanViens.ToListAsync();
            var MaxDate = getNhanVien.Select(e => e.Date).Max();
            var MinDate = MaxDate.AddMonths(-6);

            for (var date = MinDate; date <= MaxDate; date = date.AddMonths(1))
            {
                var getCount = getNhanVien.Where(e => e.Date.Month == date.Date.Month && e.Date.Year == date.Date.Year).Count();
                var list = new ListMonth()
                {
                    Count = getCount,
                    Month = date.Date.Month,
                    Year = date.Date.Year,
                };
                listMonth.Add(list);
            }
            return new Response<List<ListMonth>>(listMonth);
        }

        /// <summary>
        /// Thống kê số lượng nhân viên theo phòng ban
        /// </summary>
        /// <returns></returns>
        public async Task<Response> GetNhanVienFromBoPhan()
        {
            try
            {
                var getBoPhan = await _context.BoPhans.Include(e => e.NhanViens).ToListAsync();
                List<NhanVienBoPhan> listNhanVien = new List<NhanVienBoPhan>();

                foreach (var item in getBoPhan)
                {
                    var list = new NhanVienBoPhan()
                    {
                        Name = item.Name,
                        Count = item.NhanViens.Count(),
                    };
                    listNhanVien.Add(list);           
                }
                return new Response<List<NhanVienBoPhan>>(listNhanVien);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }
        /// <summary>
        /// Tháng nào thuộc năm nào có số lượng nhân viên mới nhiều nhất
        /// </summary>
        /// <returns></returns>
        public async Task<Response> GetMaxNhanVienForMonth()
        {
            try
            {
                var max = 0;
                var Year = 0;
                var Month = 0;
                List<DateTime> listDate = new List<DateTime>();
                var getNhanVien = await _context.NhanViens.ToListAsync();
                var MaxDate = getNhanVien.Select(e => e.Date).Max();
                var MinDate = MaxDate.AddMonths(-6);

                for (var date = MinDate; date <= MaxDate; date = date.AddMonths(1))
                {
                    listDate.Add(date);
                }
                
                foreach (var item in listDate)
                {
                    var getCount = getNhanVien.Where(e => e.Date.Month == item.Month && e.Date.Year == item.Year).Count();
                    if (getCount > max)
                    {
                        max = getCount;
                        Month = item.Month;
                        Year = item.Year;
                    }                    
                }
               
                return new Response<MaxMonth>(new MaxMonth()
                {
                    Month = Month,
                    Year = Year,
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }
        #endregion
    }

}