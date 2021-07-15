using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbbData.entity
{
    [Serializable]
     public  class ShopEntity
    {
        public string Title { get; set; } //店铺名称
        public string ShopName { get; set; } //商品名称
        public  string  ContactName { get;set; }//联系人名称
        public string ContactPhone { get; set; }//联系人电话
    }
}
