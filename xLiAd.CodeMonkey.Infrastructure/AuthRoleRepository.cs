using xLiAd.CodeMonkey.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using xLiAd.DapperEx.Repository;

namespace xLiAd.CodeMonkey.Infrastructure
{
    public class AuthRoleRepository : Repository<AuthRole>, IAuthRoleRepository
    {
        public AuthRoleRepository(IConnectionHolder conn) : base(conn) { }

        /// <summary>
        /// 切片缓存示例方法 {class}为类名 {method}为方法名 {arg0}为第一个参数，如果参数是类，还支持 {arg0.TheField} 其中 TheField 为属性或字段。
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<AuthRole> GetModelsByRoleId(string roleId)
        {
            return base.Where(x => x.RoleId == roleId);
        }
    }

    public interface IAuthRoleRepository : IRepository<AuthRole>
    {
        List<AuthRole> GetModelsByRoleId(string roleId);

        /// <summary>
        /// 切片删除缓存示例方法 {class}为类名，即：清除所有名称里带"TemplProjectV2Core3:AuthRoleRepository"的缓存，若不加 KeyPattern ，默认为 类名。此处不支持参数 arg
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        new int Add(AuthRole obj);
    }
}
