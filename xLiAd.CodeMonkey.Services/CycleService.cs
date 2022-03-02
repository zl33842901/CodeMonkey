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
    [AopDlcAttribute]
    public class CycleService : CycleServiceBase,ICycleService
    {
        private readonly ICycleRepository cycleRepository;
        public CycleService(ICycleRepository cycleRepository) : base(cycleRepository.CountAll)
        {
            this.cycleRepository = cycleRepository;
        }

        public async Task<IApiResultModel> GetPageListAsync(PageQueryDto queryDto)
        {
            Expression<Func<Cycle, bool>> expression;
            if (queryDto.Key.NullOrEmpty())
                expression = x => true;
            else
                expression = x =>  x.CycleName == queryDto.Key || x.Status == queryDto.Key || x.CreateUserId.Contains(queryDto.Key) || x.LastUpdateUserId.Contains(queryDto.Key);

            var result = await cycleRepository.PageListAsync(expression, x => x.CycleId, queryDto.PageIndex, queryDto.PageSize, true);
            return ApiListResultModel.FromPageData(result.Items, result.Total, result.PageIndex, result.PageSize);
        }

        public async Task<IApiResultModel> AddAsync(CycleEditDto model)
        {
            var entity = MappingHelper.Mapper.Map<Cycle>(model);
            var id = await cycleRepository.AddAsync(entity);
            return ApiResultModel.FromData(true);
        }

        public async Task<IApiResultModel> EditAsync(CycleEditDto model)
        {
            var entity = MappingHelper.Mapper.Map<Cycle>(model);
            entity.LastUpdateTime = DateTime.Now;
            var result = await cycleRepository.UpdateAsync(entity);
            return ApiResultModel.FromData(result > 0);
        }
        public async Task<IApiResultModel> DeleteAsync(int id)
        {
            var result = await cycleRepository.DeleteAsync(id);
            return ApiResultModel.FromData(true);
        }
        public void VoidMethod(int a)
        {

        }
        public string Abc { get; set; }

        private string PrivateMethod() { return "a"; }

        protected string ProtectedMethod() { return "b"; }

        internal string InternalMethod() { return "c"; }

        public static string StaticMethod(out int c) { c = 5; return string.Empty; }

        public static string fieldStaticString = "abc";

        public string fieldString = "abc";
    }

    public class CycleServiceBase
    {
        public CycleServiceBase(int a) { }
        public int hahaha() => 5;
    }

    public interface ICycleService
    {
        Task<IApiResultModel> GetPageListAsync(PageQueryDto queryDto);
        Task<IApiResultModel> AddAsync(CycleEditDto model);
        Task<IApiResultModel> EditAsync(CycleEditDto model);
        Task<IApiResultModel> DeleteAsync(int id);
        void VoidMethod(int a);

        string Abc { get; set; }
    }
}