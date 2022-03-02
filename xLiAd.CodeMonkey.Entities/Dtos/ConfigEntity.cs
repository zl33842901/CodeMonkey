using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLiAd.CodeMonkey.Entities.Dtos
{
    /// <summary>
    /// 配置实体类
    /// </summary>
    public class ConfigEntity : IConfigEntity
    {
        public string SqlConnectionString { get; set; }
        public string ExceptionLogCenterUrl { get; set; }
        public bool? AspectLog { get; set; }
        public string ExceptionLogCenterClient { get; set; }
        public string ExceptionLogCenterEnv { get; set; }
        public string SSOInterfaceUrl { get; set; }
        public string LoginUrl { get; set; }
        public bool EnableCache { get; set; }
        public string CacheType { get; set; }
        public string RedisUrl { get; set; }
        public int CacheTimeoutMinute { get; set; }
        public string RedisPrefix { get; set; }
        public string ExcelExportUrl { get; set; }
        public string MinioUrl { get; set; }
        public string MinioBucketName { get; set; }
        public string MinioAccessKey { get; set; }
        public string MinioSecretKey { get; set; }
        public string MinioImageProxyUrl { get; set; }
    }

    public interface IConfigEntity
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        string SqlConnectionString { get; }
        /// <summary>
        /// ELC地址
        /// </summary>
        string ExceptionLogCenterUrl { get; }
        /// <summary>
        /// 是否开启ELC接口记录
        /// </summary>
        bool? AspectLog { get; }
        /// <summary>
        /// ELC客户端名称
        /// </summary>
        string ExceptionLogCenterClient { get; }
        /// <summary>
        /// ELC客户端环境
        /// </summary>
        string ExceptionLogCenterEnv { get; }
        /// <summary>
        /// SSO 接口地址
        /// </summary>
        string SSOInterfaceUrl { get; }
        /// <summary>
        /// 登录页地址（API项目不用）
        /// </summary>
        string LoginUrl { get; }
        /// <summary>
        /// 是否开启缓存
        /// </summary>
        bool EnableCache { get; }
        /// <summary>
        /// 缓存类型 Redis Memory
        /// </summary>
        string CacheType { get; }
        /// <summary>
        /// Redis 地址
        /// </summary>
        string RedisUrl { get; }
        /// <summary>
        /// 缓存生命周期（分钟）
        /// </summary>
        int CacheTimeoutMinute { get; }
        /// <summary>
        /// Redis 键前辍
        /// </summary>
        string RedisPrefix { get; }
        /// <summary>
        /// EXCEL导出地址
        /// </summary>
        string ExcelExportUrl { get; }
        /// <summary>
        /// minio 地址
        /// </summary>
        string MinioUrl { get; }
        /// <summary>
        /// minio 存储桶名称
        /// </summary>
        string MinioBucketName { get; }
        /// <summary>
        /// minio 用户名
        /// </summary>
        string MinioAccessKey { get; }
        /// <summary>
        /// minio 密码
        /// </summary>
        string MinioSecretKey { get; }
        /// <summary>
        /// minio 图片缩略图地址
        /// </summary>
        string MinioImageProxyUrl { get; }
    }
}
