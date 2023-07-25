using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuaVatTu.Business;
using MuaVatTu.Common;
using MuaVatTu.Common.Helpers;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MuaVatTu.API.v1
{
    [ApiController]
    [Route("api/news")]
    [AllowAnonymous]
    public class NewController : ControllerBase
    {
        private readonly INewHandler _newHandler;

        public NewController(INewHandler newHandler)
        {

            _newHandler = newHandler;   
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
        public async Task<IActionResult> GetFilterAsync([FromQuery] string prameter = "", [FromQuery]int page = 1, [FromQuery] int size = 20, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            // Call service
            var filterObject = JsonConvert.DeserializeObject<NewQueryModel>(filter);

            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;
            filterObject.FullTextSearch = prameter;
            var result = await _newHandler.Get(filterObject);
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
            var result = await _newHandler.GetById(id);
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
        public async Task<IActionResult> CreateAsync( [FromBody] AddNewModel filter)
        {
            var result = await _newHandler.Create(filter);
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
            var result = await _newHandler.CreateData();
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
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateNewModel filter)
        {
            var result = await _newHandler.Update(id, filter);

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
            var result = await _newHandler.Delete(id);

            // Hander response
            return Helper.TransformData(result);
        }

        #endregion
    }
}
