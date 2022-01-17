using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlbbData.entity;
using WindowsFormsApp.publishSubscribe;

namespace ArchitectureDesignSafety
{
    public class Global
    {
        public static string FirstUrl = "https://www.dianping.com/";
        public static string LoginUrl = "https://account.dianping.com/login?redir=https://www.dianping.com";
        public static string special = "1";
        public static List<ShopEntity> DateList = new List<ShopEntity>();
        public static List<ShopListInfoEntity> ResultList = new List<ShopListInfoEntity>();

        public static List<BaggageHandler> providers = new List<BaggageHandler>();//发布消息对象

        public static List<ListUrlEntity> AccomplisCrawlingUrl = new List<ListUrlEntity>();//之前完成爬取的数据Url
        public static Dictionary<string,List<CodeInfo>> CodeDictionary = new Dictionary<string, List<CodeInfo>>();
        public static string CodeDictionaryKey = "";
        //public static string JsonKey = "AccomplisCrawlingUrl";
        //public static string fontDir = "font";
        //public static Dictionary<string, string> fontNumber = new Dictionary<string, string>(){{"f86b","1"},{"e19e","2"},{"e9d4","3"},{"e6b2","4"},{"ef53","5"},{"e16f","6"},{"ee00","7"},{"f46b","8"},{"ee9f","9"},{"eaf2","0"},{"0078","、"}};
        internal static string mysqlServer = "server=localhost;port=3306;uid=root;pwd=123456;database=dzdp;";
    }
}
