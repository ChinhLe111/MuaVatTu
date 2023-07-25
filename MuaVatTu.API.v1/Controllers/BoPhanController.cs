using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MuaVatTu.Business;
using MuaVatTu.Common;
using MuaVatTu.Data;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MuaVatTu.API.v1
{
    [ApiController]
    [Route("api/bophans")]
    [AllowAnonymous]
    public class BoPhanController : ControllerBase
    {
        private readonly IBoPhanHandler _boPhanHandler;
        private readonly IMongoCollection<BoPhanMongoDB> _boPhanCollection;

        public BoPhanController(IBoPhanHandler boPhanHandler)
        {
            _boPhanHandler = boPhanHandler;
            var DatabaseName = "MuaVatTu";
            var CollectionName = "mvt_BoPhan";
            var ConnectionString = "mongodb://localhost:27017/";

            var mongoUrl = MongoUrl.Create(ConnectionString);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(DatabaseName);
            _boPhanCollection = database.GetCollection<BoPhanMongoDB>(CollectionName);
        }

        #region CRUD
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpGet]
        public async Task<IActionResult> GetFilterAsync([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            // Call service
            var filterObject = JsonConvert.DeserializeObject<BoPhanQueryModel>(filter);

            sort = "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;
            var result = await _boPhanHandler.Get(filterObject);
            // Hander response
            return Helper.TransformData(result);
        }
        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _boPhanHandler.GetById(id);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Thêm dữ liệu
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpPost("SeedData")]
        public async Task<IActionResult> CreateAsync()
        {
            var result = await _boPhanHandler.CreateData();
            // Hander response
            return Ok();
        }
        /// <summary>
        /// Kết quả
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync(BoPhanAddModel filter)
        {
            var result = await _boPhanHandler.Create(filter);
                 // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [ HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] BoPhanUpdateModel filter)
        {
            var result = await _boPhanHandler.Update(id, filter);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _boPhanHandler.Delete(id);

            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("phong-ban-dang-ky-nhieu-san-pham-nhat")]
        public async Task<IActionResult> GetBoPhan()
        {
            var result = await _boPhanHandler.GetBoPhan();
            // Hander response
            return Helper.TransformData(result);
        }
        #endregion

        #region MongoDB
        [HttpGet("MongoDB/Pagination/boPhans")]
        public IActionResult PaginationBoPhanMongoDB([FromQuery] string parameter, [FromQuery] string sort, [FromQuery] int page)
        {
            return Ok(_boPhanHandler.Query(parameter, sort, page));
        }
        /// <summary>
        /// Post
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpPost("MongoDB/boPhans")]
        public async Task<ActionResult> CreateMongoBoPhan(BoPhanMongoDB boPhanMongoDB)
        {
            await _boPhanCollection.InsertOneAsync(boPhanMongoDB);
            return Ok();
        }
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpPut("MongoDB/boPhans")]
        public async Task<ActionResult> UpdateMongoBoPhan(BoPhanMongoDB boPhanMongoDB)
        {
            var filter = Builders<BoPhanMongoDB>.Filter.Eq(x => x.Id, boPhanMongoDB.Id);
            await _boPhanCollection.ReplaceOneAsync(filter, boPhanMongoDB);
            return Ok();
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpDelete("MongoDB/boPhans")]
        public async Task<ActionResult> DeleteMongoBoPhan(string Id)
        {
            var filter = Builders<BoPhanMongoDB>.Filter.Eq(x => x.Id, Id);
            await _boPhanCollection.DeleteOneAsync(filter);
            return Ok();
        }
        #endregion
    }
}
