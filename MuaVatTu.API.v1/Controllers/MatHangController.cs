using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuaVatTu.Business;
using MuaVatTu.Common;
using MuaVatTu.Common.Helpers;
using MuaVatTu.Data;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MuaVatTu.API.v1
{
    [ApiController]
    [Route("api/mathangs")]
    [AllowAnonymous]
    public class MatHangController : ControllerBase
    {
        private readonly IMatHangHandler _matHangHandler;

        public MatHangController(IMatHangHandler matHangHandler)
        {

            _matHangHandler = matHangHandler;
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
            var filterObject = JsonConvert.DeserializeObject<MatHangQueryModel>(filter);

            sort = "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;
            var result = await _matHangHandler.Get( filterObject);
            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpPost]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync(Guid dangKyId, AddMatHangModel filter)
        {
            var result = await _matHangHandler.Create(dangKyId, filter);
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
        [HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateMatHangModel filter)
        {
            var result = await _matHangHandler.Update(id, filter);

            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Get By Id
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetById( Guid id)
        {
            var result = await _matHangHandler.GetById(id);
            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpGet("san-pham-dang-ky-nhieu-nhat")]
        public async Task<IActionResult> GetMaxCount()
        {
            var result = await _matHangHandler.GetMaxCount();
            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Lấy thông tin mặt hàng đã đăng ký
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpGet, Route("{dangkyid}")]
        public async Task<IActionResult> GetByDangKyId(Guid dangKyId, Guid id)
        {
            var result = await _matHangHandler.GetByDangKyId(dangKyId, id);
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
        public async Task<IActionResult> DeleteAsync(Guid dangKyId)
        {
            var result = await _matHangHandler.Delete(dangKyId);

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
        public async Task<IActionResult> CreateAsync(Guid dangKyId)
        {
            var result = await _matHangHandler.CreateData(dangKyId);
            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpGet("thang-nam-dang-ky-nhieu-nhat")]
        public async Task<IActionResult> GetMonthMaxCount()
        {
            var result = await _matHangHandler.GetMonthMaxCount();
            // Hander response
            return Helper.TransformData(result);
        }
        #endregion
    }
}
