using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace AlbbData.entity
{
    [SugarTable("shopListInfo", "店铺列表页信息表", IsDisabledUpdateAll = true)]
    public class ShopListInfoEntity
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Name { get; set; }
        /// <summary>
        /// 是否完成详细页面的检索，1完成，0未完成
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool isAccomplishNavDetail { get; set; }
        /// <summary>
        /// 详细页面导航
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string DetailNavUrl { get; set; }//详情页导航地址
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 店铺类型
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ShopType { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string City { get; set; }
    }
}
