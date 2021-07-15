using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbbData.entity
{
    
    [Serializable]
    public class ListUrlEntity
    {
        public string UrlAddress { get; set; }
        public string Name{get;set;}

        public bool isAccomplishSearchingList { get; set; }
        public List<ListUrlEntity> ChildrenEntityList{get;set;}
    }
}
