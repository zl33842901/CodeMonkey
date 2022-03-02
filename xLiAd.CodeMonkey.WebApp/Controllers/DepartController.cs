using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using xLiAd.CodeMonkey.Entities;
using xLiAd.CodeMonkey.Services;
using xLiAd.CodeMonkey.Entities.Dtos;
using xLiAd.CodeMonkey.Entities.QueryDtos;
using System.Threading.Tasks;

namespace xLiAd.CodeMonkey.WebApp.Controllers
{
    //[Authorize]
    public class DepartController : Controller
    {
        private readonly IDepartService departService;
        public DepartController(IDepartService departService)
        {
            this.departService = departService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IApiResultModel> Add([FromBody]DepartEditDto dto)
        {
            return await departService.AddAsync(dto);
        }

        [HttpPost]
        public async Task<IApiResultModel> Edit([FromBody]DepartEditDto dto)
        {
            return await departService.EditAsync(dto);
        }

        [HttpPost]
        public async Task<IApiResultModel> Delete(int id)
        {
            return await departService.DeleteAsync(id);
        }

        [HttpPost]
        public async Task<IApiResultModel> GetListData(PageQueryDto queryDto)
        {
            var result = await departService.GetPageListAsync(queryDto);
            return result;
        }
    }
}