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
    [Route("api/nhanViens")]
    [AllowAnonymous]
    public class NhanVienController : ControllerBase
    {
        private readonly INhanVienHandler _nhanVienHandler;

        public NhanVienController(INhanVienHandler nhanVienHandler)
        {

            _nhanVienHandler = nhanVienHandler;   
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
            var filterObject = JsonConvert.DeserializeObject<NhanVienQueryModel>(filter);

            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;
            var result = await _nhanVienHandler.Get(filterObject);
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
            var result = await _nhanVienHandler.GetById(id);
            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Tính tổng nhân viên từ phòng ban
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpGet("thong-ke-nhan-vien-theo-phong-ban")]
        public async Task<IActionResult> GetNhanVienFromBoPhan()
        {
            var result = await _nhanVienHandler.GetNhanVienFromBoPhan();
            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Tính tổng nhân viên theo tháng
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpGet("thong-ke-nhan-vien-theo-thang")]
        public async Task<IActionResult> GetNhanVienForMonth()
        {
            var result = await _nhanVienHandler.GetNhanVienForMonth();
            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Tính tổng nhân viên theo tháng
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpGet("thang-nam-co-nhan-vien-moi-nhieu-nhat")]
        public async Task<IActionResult> GetSum()
        {
            var result = await _nhanVienHandler.GetMaxNhanVienForMonth();
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
        public async Task<IActionResult> CreateAsync(Guid boPhanId, [FromBody] AddNhanVienModel filter)
        {
            var result = await _nhanVienHandler.Create(boPhanId, filter);
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
            var result = await _nhanVienHandler.CreateData();
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
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateNhanVienModel filter)
        {
            var result = await _nhanVienHandler.Update(id, filter);

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
            var result = await _nhanVienHandler.Delete(id);

            // Hander response
            return Helper.TransformData(result);
        }

        #endregion
    }
}
