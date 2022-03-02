using xLiAd.CodeMonkey.Entities;
using xLiAd.CodeMonkey.Entities.Dtos;
using xLiAd.CodeMonkey.Entities.QueryDtos;
using xLiAd.CodeMonkey.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xLiAd.CodeMonkey.WebApp.Controllers
{
    //[Authorize]
    public class AuthRoleController : Controller
    {
        private readonly IAuthRoleService authRoleService;
        public AuthRoleController(IAuthRoleService authRoleService)
        {
            this.authRoleService = authRoleService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IApiResultModel Add(AuthRole authRole)
        {
            return authRoleService.Add(authRole);
        }

        [HttpPost]
        public IApiResultModel Edit(AuthRole authRole)
        {
            return authRoleService.Edit(authRole);
        }

        [HttpPost]
        public IApiResultModel Delete(int id)
        {
            return authRoleService.Delete(id);
        }

        [HttpPost]
        public IApiResultModel GetListData(PageQueryDto queryDto)
        {
            var result = authRoleService.GetPageList(queryDto);
            return result;
        }
    }
}
