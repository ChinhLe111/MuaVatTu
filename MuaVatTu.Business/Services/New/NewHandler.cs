using AutoMapper;
using Bogus;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using MuaVatTu.Common;
using MuaVatTu.Data;
using Serilog;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Code = System.Net.HttpStatusCode;

namespace MuaVatTu.Business
{
    public class NewHandler : INewHandler
    {
        private readonly MuaVatTuContext _context;
        private readonly IMapper _mapper;
        public NewHandler(MuaVatTuContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #region CRUD
        public async Task<Response> Get(NewQueryModel filter)
        {
            var NewFromRepo = await _context.News.FirstOrDefaultAsync();
            if (NewFromRepo == null)
                return new Response(Code.OK, "Không tồn tại");
            try
            {
                var predicate = BuildQuery(filter);
                var result = await _context.News.Where(predicate).GetPageAsync(filter);
                var data = _mapper.Map<Pagination<New>, Pagination<NewDto>>(result);
                return new ResponsePagination<NewDto>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }
        public async Task<Response> Create(AddNewModel filter)
        {
            try
            {
                var New = _mapper.Map<New>(filter);
                New.UnaccentTitle = Common.Utils.RemoveVietnameseSign(New.Title).Replace(' ', '-');
                _context.Add(New);

                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    var data = _mapper.Map<NewDto>(New);
                    return new ResponseObject<NewDto>(data, "Thêm mới thành công");
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
            var NewFromRepo = await _context.News.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (NewFromRepo == null)
                return new Response(Code.OK, "Không tồn tại");

            _context.News.Remove(NewFromRepo);
            var status = await _context.SaveChangesAsync();
            return new Response(Code.OK, "Xóa thành công");
        }
        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var entity = await _context.News.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (entity == null)
                    return new ResponseError(Code.NotFound, "Không tìm thấy tin tức");

                var data = _mapper.Map<New, NewDto>(entity);
                return new ResponseObject<NewDto>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }

        }
        public async Task<Response> Update(Guid id, UpdateNewModel model)
        {
            try
            {

                var entityModel = await _context.News.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (entityModel == null)
                    return new ResponseError(Code.NotFound, "Không tìm thấy tin tức");

                _mapper.Map(model, entityModel);
                entityModel.UnaccentTitle = Common.Utils.RemoveVietnameseSign(model.Title).Replace(' ', '-');

                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    var data = _mapper.Map<NewDto>(entityModel);
                    return new ResponseObject<NewDto>(data, "Cập nhật thành công");

                }

                return new ResponseError(Code.NotFound, "Cập nhật thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }

        }
        public Expression<Func<New, bool>> BuildQuery(NewQueryModel query)
        {
            var predicate = PredicateBuilder.New<New>(true);

            if (query.Id.HasValue && query.Id.Value != Guid.Empty)
            {
                predicate = predicate.And(s => s.Id == query.Id);
            }

            foreach (var item in Common.Utils.AddUnicode(query.FullTextSearch))
            {
                if (!string.IsNullOrEmpty(query.FullTextSearch))
                    predicate.And(s => s.SearchVector.Matches(Common.Utils.RemoveVietnameseSign(query.FullTextSearch).Replace('-', ' ')));
            }

            return predicate;
        }

        public Task<Response> CreateData()
        {
            var newFaker = new Faker<New>()
             .RuleFor(x => x.Title, x => x.Company.CompanyName())
             .RuleFor(x => x.Slug, x => x.Lorem.Slug(5)) 
             .RuleFor(x => x.Content, x => x.Company.CompanyName())
             .RuleFor(x => x.CreatedOnDate, x => x.Date.Recent(31));

            _context.AddRange(newFaker.Generate(1000));
            _context.SaveChanges();

            return null;
        }
        #endregion
    }
}