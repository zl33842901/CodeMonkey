using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using xLiAd.CodeMonkey.Entities;
using xLiAd.CodeMonkey.Infrastructure;
using xLiAd.CodeMonkey.Entities.Dtos;
using xLiAd.CodeMonkey.Entities.QueryDtos;
using xLiAd.CodeMonkey.Services.Assets;
using System.Threading.Tasks;

namespace xLiAd.CodeMonkey.Services
{
    public class DepartService : IDepartService
    {
        private readonly IDepartRepository departRepository;
        public DepartService(IDepartRepository departRepository)
        {
            this.departRepository = departRepository;
        }
        [AopDlc]
        public async Task<IApiResultModel> GetPageListAsync(PageQueryDto queryDto)
        {
            Expression<Func<Depart, bool>> expression;
            if (queryDto.Key.NullOrEmpty())
                expression = x => x.Deleted == false;
            else
                expression = x =>  x.Name == queryDto.Key || x.NameChain == queryDto.Key || x.ShadowChain.Contains(queryDto.Key) || x.AllSubIds.Contains(queryDto.Key);

            var result = await departRepository.PageListAsync(expression, x => x.Id, queryDto.PageIndex, queryDto.PageSize, true);
            return ApiListResultModel.FromPageData(result.Items, result.Total, result.PageIndex, result.PageSize);
        }

        public async Task<IApiResultModel> AddAsync(DepartEditDto model)
        {
            var entity = MappingHelper.Mapper.Map<Depart>(model);
            var id = await departRepository.AddAsync(entity);
            return ApiResultModel.FromData(true);
        }

        public async Task<IApiResultModel> EditAsync(DepartEditDto model)
        {
            var entity = MappingHelper.Mapper.Map<Depart>(model);
            var result = await departRepository.UpdateAsync(entity);
            return ApiResultModel.FromData(result > 0);
        }

        public async Task<IApiResultModel> DeleteAsync(int id)
        {
            var result = await departRepository.UpdateWhereAsync(x => x.Id == id, x => x.Deleted, true);
            return ApiResultModel.FromData(true);
        }

    }

    public interface IDepartService
    {
        Task<IApiResultModel> GetPageListAsync(PageQueryDto queryDto);
        Task<IApiResultModel> AddAsync(DepartEditDto model);
        Task<IApiResultModel> EditAsync(DepartEditDto model);
        Task<IApiResultModel> DeleteAsync(int id);
    }
}