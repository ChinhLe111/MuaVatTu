using AutoMapper;
using AutoMapper.Configuration;
using Bogus;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MuaVatTu.Common;
using MuaVatTu.Common.Helpers;
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
    public class BoPhanHandler : IBoPhanHandler
    {
        private readonly IMapper _mapper;
        private readonly MuaVatTuContext _context;
        private readonly ILogger<BoPhanHandler> _logger;
        private readonly IMongoCollection<BoPhanMongoDB> _boPhanCollection;

        public BoPhanHandler(ILogger<BoPhanHandler> logger, IMapper mapper, MuaVatTuContext context)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;

            var DatabaseName = "MuaVatTu";
            var CollectionName = "mvt_BoPhan";
            var ConnectionString = "mongodb://localhost:27017/";

            var mongoUrl = MongoUrl.Create(ConnectionString);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(DatabaseName);
            _boPhanCollection = database.GetCollection<BoPhanMongoDB>(CollectionName);
        }

        #region CRUD
        public async Task<Response> Get(BoPhanQueryModel filter)
        {
            try
            {
                var predicate = BuildQuery(filter);
                var result = await _context.BoPhans.GetPageAsync(filter);
                var data = _mapper.Map<Pagination<BoPhan>, Pagination<BoPhanDto>>(result);
                return new ResponsePagination<BoPhanDto>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }

        public async Task<Response> Create(BoPhanAddModel filter)
        {
            try
            {
                var BoPhanFromRepo = await _context.BoPhans.Where(x => x.NameOfLeader == filter.NameOfLeader).FirstOrDefaultAsync();
                if (BoPhanFromRepo != null)
                    return new Response(Code.BadRequest, "Trùng tên");

                var boPhan = _mapper.Map<BoPhan>(filter);

                _context.Add(boPhan);

                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    var data = _mapper.Map<BoPhanDto>(boPhan);
                    return new ResponseObject<BoPhanDto>(data, "Thêm mới thành công");

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
            var BoPhanFromRepo = await _context.BoPhans.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (BoPhanFromRepo == null)
                return new Response(Code.NotFound, "Không tồn tại");

            _context.BoPhans.Remove(BoPhanFromRepo);
            var status = await _context.SaveChangesAsync();
            return new Response(Code.OK, "Xóa thành công");
        }

        public async Task<Response> GetById(Guid id)
        {
            try
            {

                var entity = await _context.BoPhans.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (entity == null)
                    return new ResponseError(Code.NotFound, "Không tìm thấy bộ phận");

                var data = _mapper.Map<BoPhan, BoPhanDto>(entity);
                return new ResponseObject<BoPhanDto>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }

        }

        public async Task<Response> Update(Guid id, BoPhanUpdateModel model)
        {
            try
            {
                var entityModel = await _context.BoPhans.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (entityModel == null)
                    return new ResponseError(Code.NotFound, "Không tồn tại");

                _mapper.Map(model,entityModel);

                var status = await _context.SaveChangesAsync();

                if (status > 0)
                {
                    var data = _mapper.Map<BoPhanDto>(entityModel);
                    return new ResponseObject<BoPhanDto>(data, "Cập nhật thành công");
                }

                return new ResponseError(Code.NotFound, "Cập nhật thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }

        }

        private Expression<Func<BoPhan, bool>> BuildQuery(BoPhanQueryModel query)
        {
            var predicate = PredicateBuilder.New<BoPhan>(true);

            if (query.Id.HasValue && query.Id.Value != Guid.Empty)
            {
                predicate = predicate.And(s => s.Id == query.Id);
            }

            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.NameOfLeader.ToLower().Contains(query.FullTextSearch));

            return predicate;
        }
        public Task<Response> CreateData()
        {
            var boPhanFaker = new Faker<BoPhan>()
                .RuleFor(x => x.Name, x => x.Person.UserName)
                .RuleFor(x => x.NameOfLeader, x => x.Person.FirstName)
                .RuleFor(x => x.Date, x => x.Date.Recent(31))
                .RuleFor(x => x.Number, x => x.Random.Number(1,10));

            _context.AddRange(boPhanFaker.Generate(10));
            _context.SaveChanges();

            return null;
        }

        /// <summary>
        /// Phòng ban đăng kí nhiều sản phẩm nhất (trả về phòng ban và số lượng sản phẩm đăng kí)
        /// </summary>
        /// <returns></returns>
        public async Task<Response> GetBoPhan()
        {
            var max = 0;
            BoPhanDangKyModel boPhanDangKy = new BoPhanDangKyModel();
            var getMatHang = await _context.MatHangs.Include(e => e.DangKy).ThenInclude(e => e.BoPhan).ToListAsync();
            var getDangKyId = getMatHang.Select(e => e.DangKyId).Distinct().ToList();

                foreach (var item in getDangKyId)
                {
                    var getBoPhanId = getMatHang.Where(e => e.DangKyId == item).Select(e => e.DangKy.BoPhan.Id).FirstOrDefault();
                    var sum = getMatHang.Where(e => e.DangKy.BoPhanId == getBoPhanId).Sum(e => e.Count);
                       if (sum > max)
                       {
                            boPhanDangKy.Count = sum;
                            boPhanDangKy.BoPhanId = getBoPhanId;
                            boPhanDangKy.Date = getMatHang.Where(e => e.DangKy.BoPhanId == getBoPhanId).Select(e => e.DangKy.BoPhan.Date).FirstOrDefault();
                            boPhanDangKy.Name = getMatHang.Where(e => e.DangKy.BoPhanId == getBoPhanId).Select(e => e.DangKy.BoPhan.Name).FirstOrDefault();
                            boPhanDangKy.NameOfLeader = getMatHang.Where(e => e.DangKy.BoPhanId == getBoPhanId).Select(e => e.DangKy.BoPhan.NameOfLeader).FirstOrDefault();
                       }
                }

                _logger.LogInformation("BoPhanId {boPhanId} ", boPhanDangKy.BoPhanId);

                var entityModel = new Tong
                {
                    Id = Guid.NewGuid(),
                    Count = boPhanDangKy.Count,
                    BoPhanId = boPhanDangKy.BoPhanId,
                    Date = DateTime.Now,
                };
                _context.Add(entityModel);
                _context.SaveChanges();

            return new Response<BoPhanDangKyModel>(boPhanDangKy);     
        }
        public Object Query(string parameter, string sort, int? queryPage)
        {
            var filter = Builders<BoPhanMongoDB>.Filter.Empty;

            if (!string.IsNullOrEmpty(parameter))
            {
                filter = Builders<BoPhanMongoDB>.Filter.Regex("name", new BsonRegularExpression(parameter)) |
                         Builders<BoPhanMongoDB>.Filter.Regex("nameOfLeader", new BsonRegularExpression(parameter));
            }

            var find = _boPhanCollection.Find(filter);

            if (sort == "asc")
            {
                find = find.SortBy(p => p.Number);
            }
            else if (sort == "desc")
            {
                find = find.SortByDescending(p => p.Number);
            }

            int page = queryPage.GetValueOrDefault(1) == 0 ? 1 : queryPage.GetValueOrDefault(1);
            int pageSize = 3;
            var total = find.CountDocuments();

            return new
            {
                data = find.Skip((page - 1) * pageSize).Limit(pageSize).ToList(),
                total,
                pageSize,
                page
            };
        }
        #endregion
    }

}