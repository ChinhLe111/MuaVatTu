using AutoMapper;
using Bogus;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using MuaVatTu.Common;
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

    public class MatHangHandler : IMatHangHandler
    {
        private readonly MuaVatTuContext _context;
        private readonly IMapper _mapper;
        public MatHangHandler( MuaVatTuContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #region CRUD
        public async Task<Response> Get(MatHangQueryModel filter)
        {
            try
            {
                var predicate = BuildQuery(filter);
                var result = await _context.MatHangs.GetPageAsync(filter);
                var data = _mapper.Map<Pagination<MatHang>, Pagination<MatHangDto>>(result);
                return new ResponsePagination<MatHangDto>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }

        public async Task<Response> Create(Guid dangKyId, AddMatHangModel filter)
        {
            var MatHangFromRepo = await _context.MatHangs.Where(x => x.DangKyId == dangKyId).FirstOrDefaultAsync();
            if (MatHangFromRepo == null)
                return new Response(Code.OK, "Không tồn tại");

            try
            {
                var matHang = _mapper.Map<MatHang>(filter);
                matHang.DangKyId = dangKyId;
                _context.Add(matHang);

                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    var data = _mapper.Map<MatHangDto>(matHang);
                    return new ResponseObject<MatHangDto>(data, "Thêm mới thành công");


                }

                return new ResponseError(Code.NotFound, "Thêm mới thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }

        public async Task<Response> Delete(Guid dangKyId)
        {
            var MatHangFromRepo = await _context.MatHangs.Where(x => x.DangKyId == dangKyId).FirstOrDefaultAsync();
            if (MatHangFromRepo == null)
                return new Response(Code.OK, "Không tồn tại");

            _context.MatHangs.Remove(MatHangFromRepo);
            var status = await _context.SaveChangesAsync();
            return new Response(Code.OK, "Xóa thành công");
        }

        public async Task<Response> GetByDangKyId(Guid dangKyId, Guid id)
        {
            try
            {
                var entity = await _context.MatHangs.Where(x => x.Id == id && x.DangKyId == dangKyId).FirstOrDefaultAsync();

                if (entity == null)
                    return new ResponseError(Code.NotFound, "Không tìm thấy bản ghi");

                var data = _mapper.Map<MatHang, MatHangDto>(entity);
                return new ResponseObject<MatHangDto>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }

        }
        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var entity = await _context.MatHangs.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (entity == null)
                    return new ResponseError(Code.NotFound, "Không tìm thấy mặt hàng");

                var data = _mapper.Map<MatHang, MatHangDto>(entity);
                return new ResponseObject<MatHangDto>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }

        }
        public async Task<Response> Update(Guid id, UpdateMatHangModel model)
        {
            try
            {

                var entityModel = await _context.MatHangs.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (entityModel == null)
                    return new ResponseError(Code.NotFound, "Không tìm thấy bản ghi");

                _mapper.Map(model, entityModel);

                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    var data = _mapper.Map<MatHangDto>(entityModel);
                    return new ResponseObject<MatHangDto>(data, "Cập nhật thành công");

                }

                return new ResponseError(Code.NotFound, "Cập nhật thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }

        }
        private Expression<Func<MatHang, bool>> BuildQuery(MatHangQueryModel query)
        {
            var predicate = PredicateBuilder.New<MatHang>(true);

            if (query.Id.HasValue && query.Id.Value != Guid.Empty)
            {
                predicate = predicate.And(s => s.Id == query.Id);
            }

            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.Name.Contains(query.FullTextSearch)
                || s.Name.Contains(query.FullTextSearch.ToLower()));
            return predicate;
        }
        public async Task<Response> CreateData(Guid dangKyid)
        {
            Random rnd = new Random();
            for (int i = 1; i <= rnd.Next(5, 10); i++)
            {
                    var dangKyFaker = new Faker<MatHang>()
                   .RuleFor(x => x.Name, x => x.Name.FirstName())
                   .RuleFor(x => x.Count, x => x.Random.Int(1, 20))
                   .RuleFor(x => x.DangKyId, x => x.PickRandom(dangKyid));
                 _context.AddRange(dangKyFaker.Generate(1));
            }
            _context.SaveChanges();

            return null;
        }

        /// <summary>
        /// Sản phẩm được đăng kí số lượng nhiều nhất (trả về sản phẩm và số lượng được đăng kí)
        /// </summary>
        /// <returns></returns>
        public async Task<Response> GetMaxCount()
        {
            var max = 0;
            List<MatHangMaxCount> count = new List<MatHangMaxCount>();
            var getMatHang = await _context.MatHangs.ToListAsync();

            foreach (var item in getMatHang)
            {
                var get = getMatHang.Where(e => e.Name == item.Name).Sum(e => e.Count);
                if(get >= max)
                {
                    max = get;
                    var list = new MatHangMaxCount()
                    {
                        Count = max,
                        Name = item.Name
                    };
                    count.Add(list);
                }      
  
            }

            var maxCount = count.Max(e => e.Count);

            return new Response<ListName>(new ListName()
            {
                Count = maxCount,
                Names = count.Where(e => e.Count == maxCount).Select(e => e.Name).ToList()
            });
        }

        /// <summary>
        /// Tháng nào thuộc năm nào có số lượng sản phẩm được đăng kí nhiều nhất
        /// </summary>
        /// <returns></returns>
        public async Task<Response> GetMonthMaxCount()
        {
            var max = 0 ;
            DateTime datee = new DateTime();
            var getDangKy = await _context.Dangkys.Include(e => e.MatHangs).ToListAsync();
            var MaxDate = getDangKy.Select(e => e.Date).Max();
            var MinDate = getDangKy.Select(e => e.Date).Min();

            var getMatHang = await _context.MatHangs.Include(e => e.DangKy).ToListAsync();


            for (var date = MinDate; date <= MaxDate.AddMonths(1); date = date.AddMonths(1))
            {
                    var get = getMatHang.Where(e => e.DangKy.Date.Month == date.Date.Month && e.DangKy.Date.Year == date.Date.Year).Sum(e => e.Count);
                    if (get > max)
                    {
                        max = get;
                        datee = date;
                    }            
            }

            return new Response<MonthOfMaxCount>(new MonthOfMaxCount()
            {
                count = max,
                Month = datee.Month,
                Year = datee.Year,
            });
        }

        #endregion
    }

}