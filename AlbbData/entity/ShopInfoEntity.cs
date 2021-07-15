using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbbData.entity
{
    /// <summary>
    /// 数据信息
    /// </summary>
    public class ShopInfoEntity
    {
        public  string Name{get;set;}
        public  string Phone{get;set;}
        public  string Address{get;set;}
        public string ShopType{get;set;}
        public string DiscussCount{get;set;}//评论数
        public  string CustomPrice{get;set;}//客单价
        public  string ShopScore{get;set;}//评分

        public string DetailNavUrl{get;set;}//详情页导航地址

        public  string Province{get;set;}
        public string City{get;set;}
        public  string District{get;set;}
    }
}
