using xLiAd.CodeMonkey.Entities;
using xLiAd.CodeMonkey.Entities.Dtos;
using xLiAd.CodeMonkey.Entities.QueryDtos;
using xLiAd.CodeMonkey.Infrastructure;
using xLiAd.CodeMonkey.Services.Assets;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;

namespace xLiAd.CodeMonkey.Services
{
    public class AuthRoleService : IAuthRoleService
    {
        private readonly IAuthRoleRepository authRoleRepository;
        private readonly IHttpClientFactory httpClientFactory;
        public AuthRoleService(IAuthRoleRepository authRoleRepository, IHttpClientFactory httpClientFactory)
        {
            this.authRoleRepository = authRoleRepository;
            this.httpClientFactory = httpClientFactory;
        }

        public IApiResultModel GetAll()
        {
            var result = authRoleRepository.All();
            return ApiListResultModel.FromData(result);
        }

        public IApiResultModel GetPageList(PageQueryDto queryDto)
        {
            Expression<Func<AuthRole, bool>> expression;
            if (queryDto.Key.NullOrEmpty())
                expression = x => true;
            else
                expression = x => x.RoleId == queryDto.Key || x.RoleName.Contains(queryDto.Key);

            var result = authRoleRepository.PageList(expression, x => x.Id, queryDto.PageIndex, queryDto.PageSize, true);
            return ApiListResultModel.FromPageData(result.Items, result.Total, result.PageIndex, result.PageSize);
        }

        public IApiResultModel Add(AuthRole authRole)
        {
            var id = authRoleRepository.Add(authRole);
            return ApiResultModel.FromData(true);
        }

        public IApiResultModel Edit(AuthRole authRole)
        {
            var result = authRoleRepository.Update(authRole);
            return ApiResultModel.FromData(result > 0);
        }

        public IApiResultModel Delete(int id)
        {
            var result = authRoleRepository.Delete(id);
            return ApiResultModel.FromData(true);
        }
    }

    public interface IAuthRoleService
    {
        IApiResultModel GetAll();
        IApiResultModel GetPageList(PageQueryDto queryDto);
        IApiResultModel Add(AuthRole authRole);
        IApiResultModel Edit(AuthRole authRole);
        IApiResultModel Delete(int id);
    }
}
