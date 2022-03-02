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
    public class CycleController : Controller
    {
        private readonly ICycleService cycleService;
        public CycleController(ICycleService cycleService)
        {
            this.cycleService = cycleService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IApiResultModel> Add([FromBody]CycleEditDto dto)
        {
            return await cycleService.AddAsync(dto);
        }

        [HttpPost]
        public async Task<IApiResultModel> Edit([FromBody]CycleEditDto dto)
        {
            return await cycleService.EditAsync(dto);
        }

        [HttpPost]
        public async Task<IApiResultModel> Delete(int id)
        {
            return await cycleService.DeleteAsync(id);
        }

        [HttpPost]
        public async Task<IApiResultModel> GetListData(PageQueryDto queryDto)
        {
            var result = await cycleService.GetPageListAsync(queryDto);
            return result;
        }
    }
}