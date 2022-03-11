using System;
using System.Collections.Generic;

namespace demoapi.DAL.Accounting.Models
{
    public partial class TblAuthenticate
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
        /// 密码
        /// </summary>
        public string? Password { get; set; }
        /// <summary>
        /// 令牌
        /// </summary>
        public string? Token { get; set; }
    }
}
