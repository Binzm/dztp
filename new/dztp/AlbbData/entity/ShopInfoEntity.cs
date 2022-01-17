using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace AlbbData.entity
{
    /// <summary>
    /// 数据信息
    /// </summary>
    [SugarTable("shopInfo", "店铺信息表", IsDisabledUpdateAll = true)]
    public class ShopInfoEntity
    {
        [SugarColumn(IsNullable = true)]
        public  string Name{get;set;}
        [SugarColumn(IsNullable = true)]
        public  string Phone{get;set;}
        [SugarColumn(IsNullable = true)]
        public  string Address{get;set;}
        [SugarColumn(IsNullable = true)]
        public string ShopType{get;set;}
        [SugarColumn(IsNullable = true)]
        public string DiscussCount{get;set;}//评论数
        [SugarColumn(IsNullable = true)]
        public  string CustomPrice{get;set;}//客单价
        [SugarColumn(IsNullable = true)]
        public  string ShopScore{get;set;}//评分
        [SugarColumn(IsNullable = true)]
        public string DetailNavUrl{get;set;}//详情页导航地址
        [SugarColumn(IsNullable = true)]
        public  string Province{get;set;}
        [SugarColumn(IsNullable = true)]
        public string City{get;set;}
        [SugarColumn(IsNullable = true)]
        public  string District{get;set;}
        [SugarColumn(IsNullable = true)]
        public bool isAccomplishNavDetail { get; set; }

        /// <summary>
        /// 主键id
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
    }
}
