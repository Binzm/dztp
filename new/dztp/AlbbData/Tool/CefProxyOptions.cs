using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectureDesignSafety
{
    public class CefProxyOptions
    {
        public static ChromiumWebBrowser Create(string url)
        {
            return new ChromiumWebBrowser(url);
        }
        public static ChromiumWebBrowser CreateSingleCookie(string url, string userName)
        {

            RequestContextSettings requestContextSettings = new RequestContextSettings
            {
                PersistSessionCookies = false,
                PersistUserPreferences = false,
                CachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CefSharp\\Cache_" + userName)
            };
            var browser = new ChromiumWebBrowser(url)
            {
                RequestContext = new RequestContext(requestContextSettings)
            };
            #region 暂时无用
            //string name = new Random().Next(100000).ToString();
            //var setting = new RequestContextSettings()
            //{
            //    CachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CefSharp\\Cache_" + name),
            //    PersistSessionCookies = true,
            //    PersistUserPreferences = true
            //};
            //var context = new RequestContext(setting);
            //var cookieManager = context.GetCookieManager(null);
            ////这样设置的cookie不是全局的，只有当前browser才能访问
            //cookieManager.SetCookie("domain", new Cookie
            //{
            //    Name = "cookiename",
            //    Value = "cookievalue",
            //    Path = "/"
            //});
            //var browser = new ChromiumWebBrowser(url, context); 
            #endregion
            return browser;
        }

        public static async Task EvaluateJavascript(string script, ChromiumWebBrowser Browser)
        {
            //JavascriptResponse javascriptResponse = await Browser.GetMainFrame().EvaluateScriptAsPromiseAsync(script);
            JavascriptResponse javascriptResponse = await Browser.GetMainFrame().EvaluateScriptAsync(script);
            if (!javascriptResponse.Success)
            {

            }
        }

        public static bool NextPageJs(ChromiumWebBrowser webBrowser)
        {
            Task<CefSharp.JavascriptResponse> t = webBrowser.GetBrowser().MainFrame.EvaluateScriptAsync("document.getElementsByClassName(\"page\")[0].children[document.getElementsByClassName(\"page\")[0].children.length-1].innerText");
            t.Wait();
            if (t.Result.Result != null)
            {
                string result = t.Result.Result.ToString(); 
                if (!result.Contains("下一页"))
                {
                    return false;
                }
                else
                {
                    _ = EvaluateJavascript("document.getElementsByClassName(\"page\")[0].children[document.getElementsByClassName(\"page\")[0].children.length-1].click()", webBrowser); 
                    return true;
                }
            }

            return false;
        }

        public static void SearchInputJs(ChromiumWebBrowser webBrowser,string searchContent)
        {
            _ = EvaluateJavascript($"document.getElementById(\"alisearch-keywords\").value = \"{searchContent}\"", webBrowser); //收藏
            _ = EvaluateJavascript("document.getElementById(\"alisearch-submit\").click()", webBrowser); //收藏
        }

        public static void NextFirstWebPageJs(ChromiumWebBrowser webBrowser)
        {
            _ = EvaluateJavascript("document.getElementsByClassName('m-pagination-page')[0].childNodes[8].childNodes[0].click()", webBrowser);
        }

        public static void NextFirstZhengfuWebPageJs(ChromiumWebBrowser webBrowser)
        {
            _ = EvaluateJavascript("document.getElementById('next').click()", webBrowser);
        }

        public static void NextSecondWebPageJs(ChromiumWebBrowser webBrowser)
        {
            _ = EvaluateJavascript("document.getElementsByClassName('js-page-next')[0].className", webBrowser); //收藏
        }

        public static bool SecondWebPage(ChromiumWebBrowser webBrowser)
        {
            Task<CefSharp.JavascriptResponse> t = webBrowser.GetBrowser().MainFrame.EvaluateScriptAsync("document.getElementsByClassName(\"context\")[0].getElementsByTagName(\"li\")[1].innerText");
            t.Wait();
            if (t.Result.Result != null)
            {
                string result = t.Result.Result.ToString(); //js-page-next js-page-action ui-pager ui-pager-disabled
                if (result.Contains("下一节：没有了"))
                //if (result.Contains("下一节：4.2 甲、乙、丙类液体储罐（区）的防火间距"))
                {
                    return false;
                }
                else
                {
                    _ = EvaluateJavascript("document.getElementsByClassName('context')[0].getElementsByTagName('li')[1].getElementsByTagName('A')[0].click()", webBrowser); //收藏
                    return true;
                }
            }
            return false;
        }
    }
}
