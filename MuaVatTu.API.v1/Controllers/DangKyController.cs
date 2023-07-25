using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuaVatTu.Business;
using MuaVatTu.Common;
using MuaVatTu.Common.Helpers;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MuaVatTu.API.v1
{
    [ApiController]
    [Route("api/dangkys")]
    [AllowAnonymous]
    public class DangKyController : ControllerBase
    {
        private readonly IDangKyHandler _dangKyHandler;

        public DangKyController(IDangKyHandler dangKyHandler)
        {

            _dangKyHandler = dangKyHandler;   
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
            var filterObject = JsonConvert.DeserializeObject<DangKyQueryModel>(filter);

            sort = "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;
            var result = await _dangKyHandler.Get(filterObject);
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
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _dangKyHandler.GetById(id);
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
        public async Task<IActionResult> CreateAsync([FromBody] DangKyCreateModel filter)
        {
            var result = await _dangKyHandler.Create(filter);
                // Hander response
            return Helper.TransformData(result);

        }
        /// <summary>
        /// Tính tổng đơn đăng ký theo tháng
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpGet("thong-ke-dang-ky-theo-thang")]
        public async Task<IActionResult> GetDangKy()
        {
            var result = await _dangKyHandler.GetDangKyForMonth();
            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpPost("dangKyId")]
        public async Task<IActionResult> CreateProductAsync(Guid dangKyId, [FromBody] MatHangDto filter)
        {
            var result = await _dangKyHandler.CreateProduct(dangKyId, filter);
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
            var result = await _dangKyHandler.CreateData();
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
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] DangKyUpdateModel filter)
        {
            var result = await _dangKyHandler.Update(id, filter);

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
            var result = await _dangKyHandler.Delete(id);

            // Hander response
            return Helper.TransformData(result);
        }

        [HttpDelete("dangKyId")]
        public async Task<IActionResult> DeleteProductAsync(Guid dangKyId, Guid id)
        {
            var result = await _dangKyHandler.DeleteProduct(dangKyId, id);

            // Hander response
            return Helper.TransformData(result);
        }
        #endregion
    }
}
