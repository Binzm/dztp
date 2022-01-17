using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WindowsFormsApp.Tool;
using AlbbData.entity;
using ArchitectureDesignSafety;

namespace AlbbData.Tool
{
    public class DZDPCode
    {

        public static string MatchingCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return "";
            }

            if (Global.CodeDictionary == null)
            {
                return "";
            }

            code = RemoveNotUserChar(code).ToLower();
            var resultStr = "";
            if (!string.IsNullOrEmpty(Global.CodeDictionaryKey))
            {
                var itemlist = Global.CodeDictionary[Global.CodeDictionaryKey];
                if (itemlist.Find(f => f.Code == code) != null)
                {
                    var matchCodeInfo = itemlist.Find(f => f.Code == code);
                    resultStr = matchCodeInfo.Result;
                    return resultStr;
                }
                else
                {
                    foreach (var varItem in Global.CodeDictionary)
                    {
                        var key = varItem.Key;
                        var itemListSecond = varItem.Value;
                        if (itemListSecond.Find(f => f.Code == code) != null)
                        {
                            var matchCodeInfo = itemListSecond.Find(f => f.Code == code);
                            resultStr = matchCodeInfo.Result;
                            Global.CodeDictionaryKey = key;
                            return resultStr;
                        }
                    }

                }
            }

            foreach (var varItem in Global.CodeDictionary)
            {
                var key = varItem.Key;
                var itemlist = varItem.Value;
                if (itemlist.Find(f => f.Code == code) != null)
                {
                    var matchCodeInfo = itemlist.Find(f => f.Code == code);
                    resultStr = matchCodeInfo.Result;
                    Global.CodeDictionaryKey = key;
                    return resultStr;
                }
            }

            return resultStr;
        }

        /// <summary>
        /// 读取所有的字体转换文件
        /// </summary>
        public static void ReadAllTExtCodeInfo()
        {
            string projectPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            projectPath = $"{projectPath}\\font";
            var allTxtPaht = FileUtilHelper.GetFileNames(projectPath, "*.txt", false);
            if (allTxtPaht.Length > 0)
            {
                for (int i = 0; i < allTxtPaht.Length; i++)
                {
                    var codeInfoList = ReadTextCodeInfo(allTxtPaht[i]);
                    if (codeInfoList.Count > 0)
                    {
                        string fileName = GetFileName(allTxtPaht[i]);
                        if (!Global.CodeDictionary.Keys.Contains(fileName))
                        {
                            Global.CodeDictionary.Add(fileName, codeInfoList);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取文件名称
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string GetFileName(string FilePath)
        {
            string filename = System.IO.Path.GetFileName(FilePath);//文件名  “Default.aspx”
            filename = filename.Substring(0, filename.IndexOf("."));
            return filename;
        }

        /// <summary>
        /// 读取单个字体转换表
        /// </summary>
        /// <param name="tempDirAbsolutePath"></param>
        /// <returns></returns>
        public static List<CodeInfo> ReadTextCodeInfo(string tempDirAbsolutePath)
        {
            if (!FileUtilHelper.IsExistFile(tempDirAbsolutePath))
                return null;
            List<CodeInfo> resultCodeInfos = new List<CodeInfo>();
            string codeStr = ConvertJson.ReadTxtContent(tempDirAbsolutePath);
           var codeInfoList =  Regex.Split(codeStr, ",", RegexOptions.IgnoreCase).ToList();
           if (codeInfoList.Count > 0)
           {
               for (int i = 0; i < codeInfoList.Count; i++)
               {
                   string temp = codeInfoList[i];
                   var tempCodeInfo = new CodeInfo()
                   {
                       Code = temp.Substring(0, temp.IndexOf(":")),
                       Result = temp.Substring(temp.IndexOf(":"))
                   };
                   tempCodeInfo.Code = RemoveNotUserChar(tempCodeInfo.Code);
                   tempCodeInfo.Result = RemoveNotUserChar(tempCodeInfo.Result);
                   resultCodeInfos.Add(tempCodeInfo);
               }
           }

           return resultCodeInfos;
        }

        /// <summary>
        /// 替换无用字符串
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string RemoveNotUserChar(string code)
        {
            code = code.Replace("'", "").Replace("{", "").Replace("}", "").Replace("&#x", "").Replace(";", "").Replace(":", "").Trim();
            return code;
        }
    }
}
