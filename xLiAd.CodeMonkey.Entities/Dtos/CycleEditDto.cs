using System;
using System.Collections.Generic;

namespace xLiAd.CodeMonkey.Entities.Dtos
{
    /// <summary>
    /// 广告位主表
    /// </summary>
    public class CycleEditDto
    {
        #region DTO
        /// <summary>
        /// 目标周期主键ID
        /// </summary>
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
        #endregion
    }
}