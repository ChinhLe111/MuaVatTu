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
    public class DangKyHandler : IDangKyHandler
    {
        private readonly IMapper _mapper;
        private readonly MuaVatTuContext _context;
        public DangKyHandler( IMapper mapper, MuaVatTuContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        #region CRUD
        public async Task<Response> Get(DangKyQueryModel filter)
        {    
            try
            {
                var predicate = BuildQuery(filter);
                var result = await _context.Dangkys.Include(e => e.BoPhan).GetPageAsync(filter);
                var data = _mapper.Map<Pagination<DangKy>, Pagination<DangKyDto>>(result);
                return new ResponsePagination<DangKyDto>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }


        public async Task<Response> Create(DangKyCreateModel filter)
        {
            if (string.IsNullOrEmpty(filter.NameOfUser))
                throw new ArgumentNullException(nameof(filter.NameOfUser));
            try
            {
                var DangKyFromRepo = _context.Dangkys.Where(x => x.BoPhanId == filter.BoPhanId).FirstOrDefault();
                if (DangKyFromRepo == null)
                    return new Response(Code.NotFound, "Không tồn tại");

                var dangKy = _mapper.Map<DangKy>(filter);
                dangKy.Id = Guid.NewGuid();
                var listMatHang = _mapper.Map<List<MatHang>>(filter);

                foreach (var item in dangKy.MatHangs)
                {
                    var matHang = _mapper.Map<MatHang>(item);
                    item.DangKyId = dangKy.Id;
                    listMatHang.Add(matHang);
                }

                _context.Add(dangKy);

                _context.AddRange(listMatHang);

                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    var data = _mapper.Map<DangKyDto>(dangKy);
                    return new ResponseObject<DangKyDto>(data, "Thêm mới thành công");
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
            var DangKyFromRepo = _context.Dangkys.Where(x => x.Id == id).FirstOrDefault();
            if (DangKyFromRepo == null)
                return new Response(Code.NotFound, "Không tồn tại");

            _context.Dangkys.Remove(DangKyFromRepo);
            var status = await _context.SaveChangesAsync();
            return new Response(Code.OK, "Xóa thành công");
        }

        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var entity = await _context.Dangkys.Include(e => e.BoPhan).Include(e => e.MatHangs).Where(x => x.Id == id).FirstOrDefaultAsync();

                if (entity == null)
                    return new ResponseError(Code.NotFound, "Không tìm thấy bản ghi");

                var data = _mapper.Map<ICollection<MatHang>, List<MatHangDto>>(entity.MatHangs);

                return new Response<DangKyGetId>(new DangKyGetId()
                {
                    NameOfUser = entity.NameOfUser,
                    BoPhanId = entity.BoPhan.Id,
                    Date = entity.Date,
                    MatHangs = data
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }


        public async Task<Response> Update(Guid id , DangKyUpdateModel model)
        {
            try
            {
                var entityModel = await _context.Dangkys.Include(e => e.BoPhan).Where(x => x.Id == id).FirstOrDefaultAsync();

                if (entityModel == null)
                    return new ResponseError(Code.NotFound, "Không tồn tại");

                _mapper.Map(model, entityModel);

                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    var data = _mapper.Map<DangKy, DangKyDto>(entityModel);
                    return new ResponseObject<DangKyDto>(data, "Cập nhật thành công");

                }

                return new ResponseError(Code.InternalServerError, "Cập nhật thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }
        public async Task<Response> DeleteProduct(Guid dangKyId, Guid id)
        {
            var MatHangFromRepo = await _context.MatHangs.Where(x => x.Id == id && x.DangKyId == dangKyId).FirstOrDefaultAsync();
            if (MatHangFromRepo == null)
                return new Response(Code.NotFound, "Không tồn tại");

            _context.MatHangs.Remove(MatHangFromRepo);
            var status = await _context.SaveChangesAsync();
            return new Response(Code.OK, "Xóa mặt hàng thành công");
        }

        private Expression<Func<DangKy, bool>> BuildQuery(DangKyQueryModel query)
        {
            var predicate = PredicateBuilder.New<DangKy>(true);

            if (query.Id.HasValue && query.Id.Value != Guid.Empty)
            {
                predicate = predicate.And(s => s.BoPhanId == query.BoPhan.Id);
            }

            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.NameOfUser.Contains(query.FullTextSearch)); 

            return predicate;
        }

        public async Task<Response> CreateProduct(Guid dangKyId, MatHangDto filter)
        {
                if (string.IsNullOrEmpty(filter.Name))
                    throw new ArgumentNullException(nameof(filter.Name));
                try
                {     
                    var matHangEntity = new MatHang()
                    {
                        Name = filter.Name,
                        Id = Guid.NewGuid(),
                        Count = filter.Count,
                        DangKyId = dangKyId
                    };

                var getMatHang = await _context.MatHangs.Where(e => e.Name == filter.Name && e.DangKyId == dangKyId).FirstOrDefaultAsync();

                    if(getMatHang != null) 
                    {
                        getMatHang.Count = getMatHang.Count + filter.Count;
                    }
                    else
                    {
                        _context.Add(matHangEntity);
                    }
                    var status = await _context.SaveChangesAsync();

                    if (status > 0)
                    {
                        var data = _mapper.Map<MatHang, MatHangDto>(matHangEntity);
                        return new ResponseObject<MatHangDto>(data, "Thêm mới mặt hàng thành công");
                    }

                    return new ResponseError(Code.NotFound, "Thêm mới mặt hàng thất bại");
                
                }      
                catch (Exception ex)
                {
                    Log.Error(ex, string.Empty);
                    return new ResponseError(Code.InternalServerError, ex.Message);
                }
            }

        /// <summary>
        /// Thống kê số lượng đăng kí theo tháng
        /// </summary>
        /// <returns></returns>
        public async Task<Response> GetDangKyForMonth()
        {
                List<DangKyForMonth> dangKyForMonth = new List<DangKyForMonth>();
                List<DateTime> listDate = new List<DateTime>();
                var getDangKy = await _context.Dangkys.ToListAsync();
                var MaxDate = getDangKy.Select(e => e.Date).Max();
                var MinDate = MaxDate.AddMonths(-6);

            for (var date = MinDate ; date <= MaxDate; date = date.AddMonths(1))
                {
                      listDate.Add(date);
                }

                foreach (var item in listDate)
                {  
                    var getCount = getDangKy.Where(e => e.Date.Month == item.Month && e.Date.Year == item.Year).Count();
                    var list = new DangKyForMonth()
                        {
                            Count = getCount,
                            Month = item.Month,
                            Year = item.Year,                        
                        };
                        dangKyForMonth.Add(list);                   
                }
                return new Response<List<DangKyForMonth>>(dangKyForMonth);
        }

        public async Task<Response> CreateData()
        {
            var getBoPhan = await _context.BoPhans.Select(e => e.Id).ToListAsync();
            var dangKyFaker = new Faker<DangKy>()
                .RuleFor(x => x.NameOfUser, x => x.Person.FullName)
                .RuleFor(x => x.Date, x => x.Date.Recent(31))
                .RuleFor(x => x.BoPhanId, x => x.PickRandom(getBoPhan));

            _context.AddRange(dangKyFaker.Generate(1000));
            _context.SaveChanges();

            return null;
        }
        #endregion
    }
}