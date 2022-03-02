using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xLiAd.CodeMonkey.Entities
{
    /// <summary>
    /// 广告位主表
    /// </summary>
    public class Cycle
    {
        #region 字段
        /// <summary>
        /// 目标周期主键ID
        /// </summary>
        [Identity, Key]
        public int CycleId { get; set; }
        /// <summary>
        /// 目标周期名称
        /// </summary>
        public string CycleName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 是否无效，0:无效，1:有效
        /// </summary>
        public bool EnableFlag { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [AutoDateTimeWhenInsert, NoUpdate]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [NoUpdate]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        [AutoDateTimeWhenInsert]
        public DateTime LastUpdateTime { get; set; }
        /// <summary>
        /// 最后更新人
        /// </summary>
        public string LastUpdateUserId { get; set; }
        #endregion
    }
}