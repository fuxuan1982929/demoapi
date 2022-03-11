using System;
using System.Collections.Generic;

namespace demoapi.DAL.Accounting.Models
{
    public partial class TblRecord
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; } = null!;
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; } = null!;
        /// <summary>
        /// 记账日期
        /// </summary>
        //public DateOnly RecordDate { get; set; }
        public DateTime RecordDate { get; set; }
        /// <summary>
        /// 借-收入0；贷-支出1
        /// </summary>
        public int AccountType { get; set; }
        /// <summary>
        /// 类型：一般；交通；孩子；住房；通信；居家；购物；旅行；餐饮
        /// </summary>
        public string TypeName { get; set; } = null!;
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 注释
        /// </summary>
        public string? Remark { get; set; }
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
