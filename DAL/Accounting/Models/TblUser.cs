using System;
using System.Collections.Generic;

namespace demoapi.DAL.Accounting.Models
{
    public partial class TblUser
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; } = null!;
        /// <summary>
        /// 用户名
        /// </summary>
        public string? Username { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// 更新用户
        /// </summary>
        public string? UpdateUser { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 建立用户
        /// </summary>
        public string CreateUser { get; set; } = null!;
        /// <summary>
        /// 建立时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
