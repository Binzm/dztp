using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using AlbbData.entity;
using ArchitectureDesignSafety;
using CefSharp;
using CefSharp.WinForms;
using InvitationForBidsCrawler;
using WindowsFormsApp.Model;
using WindowsFormsApp.publishSubscribe;
using WindowsFormsApp.Tool;
using Newtonsoft.Json;
using System.Windows.Media;

namespace AlbbData
{
    public partial class Form1 : Form
    {
        ChromiumWebBrowser _webview;
        //private ChromiumWebBrowser _webBrowserDetial;
        private bool _isExecuteSearch;//执行搜索跳转
        //private int _initListPageNav = 0;
        private LockObj _initListPageNav = new LockObj() { Id = "00", Number = 0 };
        private LockObj _thisPageGetDate = new LockObj() { Id = "11", Number = 0, LoginCount = 0 };
        private LockObj _initDetialPageNav = new LockObj() { Id = "22", Number = 0 };
        private LockObj _taskNav = new LockObj() { Id = "33", Number = 1 };
        private List<string> _usingNavList = new List<string>();
        private int _getListAllDateCount = 0;
        private bool _beginGetDetialDate = false;
        private bool isLoad200 = true;
        private bool isSuspend = false;//是否暂停爬取数据
        private int _taskCount = 1;

        private List<ListUrlEntity> _listNav;
        private string _thisSearchTypeName;


        //发布者
        BaggageHandler provider;
        ArrivalsMonitor observer;


        public Form1()
        {

            //string getTimeResult = HttpHelp.Get("http://quan.suning.com/getSysTime.do");
            //TimeInfo BJtime = new TimeInfo();
            //if (getTimeResult != null && getTimeResult.Trim() != "")
            //{
            //    BJtime = ConvertJson.FromJSON<TimeInfo>(getTimeResult);
            //}
            //if (BJtime == null || BJtime.sysTime2 == null)
            //{
            //    BJtime = new TimeInfo() { sysTime2 = DateTime.MinValue };
            //}
            //if (BJtime.sysTime2 > Convert.ToDateTime("2021-7-16 17:55:22"))
            //{
            //    MessageBox.Show("系统异常");
            //    return;
            //}
            InitializeComponent();
            textBox1.Text = "水果";
            #region 创建发布
            provider = new BaggageHandler();
            if (!Global.providers.Exists(f => f._id == provider._id))
                Global.providers.Add(provider);
            #endregion


            #region 注册订阅者，并注册写入数据事件
            observer = new ArrivalsMonitor("BaggageClaimMonitor1");
            observer.OnChanged += Observer_OnChanged;

            #endregion
           

            InitListUrlInfo();
        }

        private void InitListUrlInfo()
        {
            _listNav = new List<ListUrlEntity>();

            #region 美食

            ListUrlEntity temp = new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/huhehaote/ch10",
                Name = "美食",
                isAccomplishSearchingList = false,
                ChildrenEntityList = new List<ListUrlEntity>()
            };
            temp.ChildrenEntityList.Add(new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/guangzhou/ch10/g110",
                isAccomplishSearchingList = false,
                Name = "火锅",
            });
            temp.ChildrenEntityList.Add(new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/guangzhou/ch10/g117",
                isAccomplishSearchingList = false,
                Name = "面包甜点",
            });
            _listNav.Add(temp);
            #endregion


        }


        /// <summary>
        /// 订阅
        /// </summary>
        private void SubScripAllPublish()
        {
            if (Global.providers != null && Global.providers.Count > 0)
            {
                Global.providers.ForEach(f =>
                {
                    observer.Subscribe(f);
                });
            }
        }

        private void Observer_OnChanged(object sender, EventArgs e)
        {
            string showMsg = sender.ToString();
            string num = showMsg.Substring(0, showMsg.IndexOf(","));
            string modelStr = showMsg.Substring(showMsg.IndexOf(",") + 1);
            ShopEntity shopEntity = ConvertJson.FromJSON<ShopEntity>(modelStr);
            if (shopEntity != null)
            {
                string showRichTextStr = $"当前时间：{DateTime.Now} -- 商品名称：{shopEntity.Title} -- 店铺名称：{shopEntity.ShopName} -- 联系人：{shopEntity.ContactName} -- 联系电话：{shopEntity.ContactPhone}";
                this.richTextBox1.Text += $"\r\n{showRichTextStr}";
            }
            //switch (num) {
            //    case "0"://代表登录
            //        //this.richTextBox1.Text += "\r\n" + $"当前时间{DateTime.Now},{showMsg.Substring(showMsg.IndexOf(",") + 1)}执行次数：{++_loginNum}";
            //        break;
            //}
        }
        private void button3_Click(object sender, EventArgs e)
        {
            _isExecuteSearch = false;
            _beginGetDetialDate = false;
            _webview = CefProxyOptions.CreateSingleCookie(Global.LoginUrl, "userInfoCookie");
            //_webview.FrameLoadEnd += Webview_LoginFrameLoadEnd;
            _webview.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(_webview);
            CheckForIllegalCrossThreadCalls = false;//允许其他线程去 操作创建窗体的销毁操作
        }

        //private void Webview_LoginFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        //{

        //}

        private void button1_Click(object sender, EventArgs e)
        {
            InitListCralIngUrl();//初始化 构造历史爬取 列表也的url数据
            InitAccomplisCrawlingUrl();//初始化 构造历史爬取完成的url数据
            string navUrl = SetThisTypeAndWriteTypeName();
            _isExecuteSearch = false;
            _beginGetDetialDate = false;
            if (_webview != null && !_webview.IsDisposed)
            {
                _webview.Load(navUrl);
            }
            else
            {
                _webview = CefProxyOptions.CreateSingleCookie(navUrl, "userInfoCookie");
            }

            
            _webview.FrameLoadEnd += Webview_FrameLoadEnd;
            _webview.Dock = DockStyle.Fill;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(_webview);
            CheckForIllegalCrossThreadCalls = false;//允许其他线程去 操作创建窗体的销毁操作
            SubScripAllPublish();
        }

        /// <summary>
        /// 设置当前对象为检索列表对象，并且真实当前的检索类型名称
        /// </summary>
        /// <param name="listUrlEntity"></param>
        private string SetThisTypeAndWriteTypeName()
        {
            for (int i = 0; i < _listNav.Count; i++)
            {
                if (!_listNav[i].isAccomplishSearchingList)
                {
                    _listNav[i].isAccomplishSearchingList = true;
                    _thisSearchTypeName = _listNav[i].Name;
                    return _listNav[i].UrlAddress;
                }
                else
                {
                    if (_listNav[i].ChildrenEntityList != null && _listNav[i].ChildrenEntityList.Count > 0)
                    {
                        for (int j = 0; j < _listNav[i].ChildrenEntityList.Count; j++)
                        {
                            if (!_listNav[i].ChildrenEntityList[j].isAccomplishSearchingList)
                            {
                                _listNav[i].ChildrenEntityList[j].isAccomplishSearchingList = true;
                                _thisSearchTypeName = _listNav[j].ChildrenEntityList[j].Name;
                                return _listNav[i].ChildrenEntityList[j].UrlAddress;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 初始化上一次 列表页面爬取的数据
        /// </summary>
        private void InitListCralIngUrl()
        {
            var fileName = textBox1.Text.Trim() + "_accomplish.txt";
            string projectPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string tempDirAbsolutePath = Path.Combine(projectPath + "\\searchResule", fileName);
            string strUrlObjStr = ConvertJson.Readjson(tempDirAbsolutePath);

            List<string> listUrlEntities = new List<string>();
            if (!string.IsNullOrEmpty(strUrlObjStr))
            {
                listUrlEntities = ConvertJson.FromJSON<List<string>>(strUrlObjStr);
            }

            if (listUrlEntities.Count > 0)
            {
                _usingNavList.Clear();
                _usingNavList = listUrlEntities;
                _getListAllDateCount = _usingNavList.Count;
                //_thisPageGetDate.Number = _usingNavList.Count;
            }
        }

        private void InitAccomplisCrawlingUrl()
        {
            var fileName = textBox1.Text.Trim() + ".txt";
            string projectPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string tempDirAbsolutePath = Path.Combine(projectPath + "\\searchResule", fileName);
            string strUrlObjStr = ConvertJson.Readjson(tempDirAbsolutePath);

            List<ListUrlEntity> listUrlEntities = new List<ListUrlEntity>();
            if (!string.IsNullOrEmpty(strUrlObjStr))
            {
                listUrlEntities = ConvertJson.FromJSON<List<ListUrlEntity>>(strUrlObjStr);
            }

            if (listUrlEntities.Count > 0)
            {
                Global.AccomplisCrawlingUrl.Clear();
                Global.AccomplisCrawlingUrl = listUrlEntities;
            }
        }

        private void Webview_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            _webview = (ChromiumWebBrowser)sender;
            if (e.HttpStatusCode.Equals(200))
            {
                ScollHeightBottomAndAsyncGetDataAndJumpNextPage();
            }
        }

        /// <summary>
        /// 继续爬取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            this.isSuspend = false;
            //if (_taskObj != null && _taskObj.Number <= 0)
            //{
            //    _taskObj.Number = 1;
            //}
            if (_webview != null && !_webview.IsDisposed)//如果列表页面检索没有结束，继续加载检索列表数据
            {
                ScollHeightBottomAndAsyncGetDataAndJumpNextPage();//
            }
            else
            {
                DealDetailAndGetDate();//检索详情页数据
            }
        }
        /// <summary>
        /// 滚动到最底下，之后获取数据
        /// </summary>
        private void ScollHeightBottomAndAsyncGetDataAndJumpNextPage()
        {
            lock (_initListPageNav)
            {
                if (_initListPageNav.Number <= 1)
                {
                    _webview.GetMainFrame().EvaluateScriptAsync("window.scrollTo(0,document.body.scrollHeight);");//滚动到最底部
                    var t1 = Task.Run(async delegate
                    {
                        await Task.Delay(3000);//页面加载完成时间
                        ScollHeightBottomAndAsyncGetDataAndJumpNextPage();
                    });
                    _initListPageNav.Number++;
                    //GetData();
                }
                else
                {
                    AsyncGetDataAndJumpNextPage();
                    _initListPageNav.Number = 0;
                }
            }

        }

        private void AsyncGetDataAndJumpNextPage()
        {
            GetData();
        }



        /// <summary>
        /// 获取列表数据
        /// </summary>
        private void GetData()
        {
            lock (_webview)
            {
                string resultStr = "";
                var task = _webview.GetSourceAsync();
                task.ContinueWith(t =>
                {
                    if (!t.IsFaulted)
                    {
                        resultStr = t.Result;
                        if (!string.IsNullOrEmpty(resultStr))
                        {
                            GetThisPageAllListDataAndGetDetialData(resultStr);
                            lock (_thisPageGetDate)
                            {
                                if (_thisPageGetDate.Number == _getListAllDateCount)
                                {
                                    _thisPageGetDate.LoginCount++;
                                }
                                else
                                {
                                    _thisPageGetDate.LoginCount = 0;
                                }
                                _thisPageGetDate.Number = _getListAllDateCount;

                                if (_thisPageGetDate.LoginCount > 4 || _thisPageGetDate.Number > 750)//刷新6次以上,数据总量没有改变，则进行详细页数据获取操作
                                //if (_thisPageGetDate.Number > 200)//测试用
                                {

                                    //if (!_beginGetDetialDate)
                                    //{
                                    //    SaveListUrlTxt();
                                    //    DealDetailAndGetDate();
                                    //    _beginGetDetialDate = true;
                                       DisposeWebviewSource(_webview);

                                    //}
                                }
                                else//跳转下一页操作
                                {
                                    ToNextPageJs(); //to next page
                                }
                            }
                          
                        }
                    }
                });
            }
        }



        private void DisposeWebviewSource(ChromiumWebBrowser webview)
        {

            var t1 = Task.Run(async delegate
            {
                await Task.Delay(10000);//页面加载完成时间
                                        //System.Threading.Thread.Sleep(3000);
                if (!webview.IsDisposed)
                {
                    try
                    {
                        webview.Dispose();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                }
            });
        }

        public void DealDetailAndGetDate()
        {
            if (!isSuspend)
            {//是否暂停爬取，默认：不暂停爬取
                string getNavStr = "";
                lock (_usingNavList)
                {
                    if (_usingNavList != null && _usingNavList.Count > 0)
                    {
                        _usingNavList = RemoveAccomplisCrawlingUrl(_usingNavList);//删除之前爬取过的url
                        getNavStr = _usingNavList.First();
                        _usingNavList.RemoveAt(0);
                        AddAccomplisCrawlingUrl(getNavStr);//将导航的url 添加到完成list中
                        Console.WriteLine($"开始导航到---》{getNavStr}");
                        CreateDetailPageWebView(getNavStr);
                    }
                }
            }
        }

        /// <summary>
        /// 将导航的url 添加到完成list中
        /// </summary>
        /// <param name="getNavStr"></param>
        private void AddAccomplisCrawlingUrl(string getNavStr)
        {
            try
            {
                lock (Global.AccomplisCrawlingUrl)
                {
                    ListUrlEntity tempEntity = new ListUrlEntity()
                    {
                        UrlAddress = getNavStr
                    };
                    Global.AccomplisCrawlingUrl.Add(tempEntity);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        /// <summary>
        /// 删除之前爬取过的url
        /// </summary>
        /// <param name="usingNavList"></param>
        /// <returns></returns>
        private List<string> RemoveAccomplisCrawlingUrl(List<string> usingNavList)
        {
            try
            {
                lock (Global.AccomplisCrawlingUrl)
                {
                    Global.AccomplisCrawlingUrl.ForEach(f =>
                    {
                        if (usingNavList.Exists(z => z.Equals(f.UrlAddress)))
                        {
                            usingNavList.Remove(f.UrlAddress);
                        }
                    });
                    return usingNavList;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public void CreateDetailPageWebView(string url)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { CreateDetailPageWebView(url); }));
                    return;
                }
                var detailWebView = CefProxyOptions.Create(url);
                detailWebView.FrameLoadEnd += DetailWebview_FrameLoadEnd;
                //detailWebView.Dock = DockStyle.Fill;
                //panel1.Controls.Clear();
                panel1.Controls.Add(detailWebView);

                var task = Task.Run(async delegate
                {
                    lock (_taskNav)
                    {
                        if (_taskNav.Number >= 0)
                        {
                            DealDetailAndGetDate();
                            _taskNav.Number--;
                        }
                    }
                });


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private void DetailWebview_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            try
            {
                var detialWebview = (ChromiumWebBrowser)sender;
                if (e.HttpStatusCode.Equals(200))
                {
                    detialWebview.GetMainFrame().EvaluateScriptAsync("document.getElementsByClassName(\"baxia-dialog-close\")[0].click();");//关闭登录窗体
                    detialWebview.GetMainFrame().EvaluateScriptAsync("document.getElementById(\"sufei-dialog-close\").click()");//关闭登录窗体
                    if (!isLoad200)
                    {
                        InsertListObj(e, detialWebview);
                    }
                    else
                    {
                        ScollDetialHeightBottomAndAsyncGetDataAndJumpNextPage(e, detialWebview);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private void ScollDetialHeightBottomAndAsyncGetDataAndJumpNextPage(FrameLoadEndEventArgs e, ChromiumWebBrowser detialWebview)
        {
            try
            {
                lock (_initDetialPageNav)
                {
                    if (_initDetialPageNav.Number <= 1)
                    {
                        if (!detialWebview.IsDisposed)
                        {
                            lock (detialWebview)
                            {

                                detialWebview.GetMainFrame().EvaluateScriptAsync("window.scrollTo(0,document.body.scrollHeight);");//滚动到最底部
                                var t1 = Task.Run(async delegate
                                {
                                    await Task.Delay(500);//页面加载完成时间
                                    ScollDetialHeightBottomAndAsyncGetDataAndJumpNextPage(e, detialWebview);
                                });
                                _initDetialPageNav.Number++;
                                if (_initDetialPageNav.Number.Equals(1))
                                {
                                    isLoad200 = false;
                                    _initDetialPageNav.Number = 0;
                                    InsertListObj(e, detialWebview);
                                }
                            }
                        }

                    }
                    else
                    {
                        InsertListObj(e, detialWebview);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        private void InsertListObj(FrameLoadEndEventArgs e, ChromiumWebBrowser detialWebview)
        {
            try
            {
                string resultStr = "";
                if (!detialWebview.IsDisposed && !e.Frame.IsDisposed)
                {
                    lock (e)
                    {
                        var task02 = e.Frame.GetSourceAsync();
                        task02.ContinueWith(t =>
                        {
                            if (!t.IsFaulted)
                            {
                                resultStr = t.Result;
                                if (!string.IsNullOrEmpty(resultStr))
                                {
                                    DateInsert(resultStr, detialWebview);
                                }
                            }
                        });
                    }

                }

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private void DateInsert(string resultStr, ChromiumWebBrowser detialWebview)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(resultStr);
            var titleSpanNode = doc.DocumentNode.SelectSingleNode("//span[@class='title-info-name']");//特殊标题
            var titleInteriorNode = doc.DocumentNode.SelectSingleNode("//h1[@class='d-title']");
            if (titleInteriorNode != null || titleSpanNode != null)
            {
                string shopName = "";
                string title = "";//商品名称
                string name = ""; //"郭忠原"
                string phone = "";//"                        18982375909\n     " 
                var shopNameNode = doc.DocumentNode.SelectSingleNode("//div[@class='nameArea']");
                var shopNameNode1 = doc.DocumentNode.SelectSingleNode("//div[@class='name has-tips']");
                if (shopNameNode != null)
                {
                    var shopCollection = shopNameNode.ChildNodes;
                    shopName = shopCollection[1].InnerHtml;//店铺名称
                }
                else if (shopNameNode1 != null)
                {
                    shopName = shopNameNode1.InnerText;
                }

                if (titleInteriorNode != null)
                {
                    title = titleInteriorNode.InnerHtml.Trim();
                }
                else
                {
                    title = titleSpanNode.InnerHtml.Trim();
                }

                var nameNode = doc.DocumentNode.SelectSingleNode("//a[@class='membername']");
                if (nameNode != null)
                {
                    name = nameNode.InnerText.Trim();
                }
                var phoneNode = doc.DocumentNode.SelectSingleNode("//dd[@class='mobile-number']");
                if (phoneNode != null)
                {
                    phone = phoneNode.InnerText.Replace("\n", "").Trim();
                }

                if (!string.IsNullOrEmpty(phone))//如果有手机号码，就保存
                {
                    ShopEntity shopEntity = new ShopEntity()
                    {
                        Title = title,
                        ShopName = shopName,
                        ContactName = name,
                        ContactPhone = phone
                    };
                    lock (Global.DateList)
                    {
                        if (Global.DateList != null && !Global.DateList.Exists(f => f.ContactName == shopEntity.ContactName && f.ContactPhone == shopEntity.ContactPhone))
                        {
                            Global.DateList.Add(shopEntity);
                            lock (provider)
                            {
                                provider.BaggageStatus(0, ConvertJson.TOString(shopEntity), 3);
                            }

                        }
                    }
                }

            }

            //var titleNode = doc.DocumentNode.SelectSingleNode("//h1[@class='d-title']");
            if (titleInteriorNode != null | titleSpanNode != null)//有标题，证明页面导航成功，可以跳转下一页
            {
                isLoad200 = true;
                DisposeWebviewSource(detialWebview);
                var task = Task.Run(async delegate
                {
                    await Task.Delay(1000);//页面加载完成时间
                    // System.Threading.Thread.Sleep(3000);
                    DealDetailAndGetDate();
                });
            }

        }

        private void ToNextPageJs()
        {

            var t = Task.Run(async delegate
                {
                    //await Task.Delay(2000);//页面加载完成时间，看网速，这里后期可能需要调整
                    //System.Threading.Thread.Sleep(3000);
                    var reslut = CefProxyOptions.NextPageJs(_webview);//如果真实跳转了下一页
                    if (reslut)
                    {
                        ScollHeightBottomAndAsyncGetDataAndJumpNextPage();//获取加载数据
                    }
                    else//开始检索下一条目录，或者检索详情页数据
                    {
                        NavNextTypeUrl();
                    }
                });
        }

        private void NavNextTypeUrl()
        {
            string navUrl = SetThisTypeAndWriteTypeName();
            if (string.IsNullOrEmpty(navUrl))//还有列表目录没有检索完成
            {
                _webview.Load(navUrl);
                _webview.FrameLoadEnd += Webview_FrameLoadEnd;
            }
            else//全部列表目录检索完成，可以检索详细页了
            {

            }
        }

        /// <summary>
        /// 整合列表数据
        /// </summary>
        /// <param name="resultStr"></param>
        private void GetThisPageAllListDataAndGetDetialData(string resultStr)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(resultStr);
            var htmlNode = doc.DocumentNode.SelectSingleNode("//div[@class='shop-list J_shop-list shop-all-list']");
            if (htmlNode != null)
            {
                HtmlAgilityPack.HtmlNodeCollection nodecollection = htmlNode.ChildNodes[1].ChildNodes;
                if (nodecollection.Count > 0)
                {
                    var htmlCity = doc.DocumentNode.SelectSingleNode("//span[@itemprop='title']");
                    for (int i = 0; i < nodecollection.Count; i++)
                    {
                        if (nodecollection[i].ChildNodes.Count > 0)
                        {
                            string numDistcussCountStr = nodecollection[1].ChildNodes[3].ChildNodes[3].ChildNodes[3].InnerHtml;//评论数处理
                            if (numDistcussCountStr != null)
                            {
                                DealNumDistcuss(ref numDistcussCountStr);
                            }
                            string priceStr = nodecollection[1].ChildNodes[3].ChildNodes[3].ChildNodes[7].InnerHtml;//人均消费处理
                            if (priceStr != null)
                            {
                                DealPriceDistcuss(ref priceStr);
                            }

                            string cityStr = "";
                            if (htmlCity != null)
                            {
                                cityStr = htmlCity.InnerText.Replace("\n","").Trim();
                                cityStr = cityStr.Replace(_thisSearchTypeName, "");
                            }

                            ShopInfoEntity tempshwoinfo = new ShopInfoEntity
                            {
                                Name = nodecollection[i].ChildNodes[3].ChildNodes[1].ChildNodes[1].ChildNodes[1].InnerText,
                                ShopScore = nodecollection[1].ChildNodes[3].ChildNodes[3].ChildNodes[1].InnerText.Replace
                                    ("\n", "").Trim(),
                                DetailNavUrl = GetHrefByAStr(nodecollection[i].ChildNodes[3].ChildNodes[1].ChildNodes[1].OuterHtml),
                                DiscussCount = numDistcussCountStr,
                               City = cityStr,
                               ShopType = _thisSearchTypeName

                            };
                            if (!Global.ResultList.Exists(f =>
                                f.Name == tempshwoinfo.Name && f.DetailNavUrl == tempshwoinfo.DetailNavUrl))
                            {
                                Global.ResultList.Add(tempshwoinfo);
                                _getListAllDateCount = Global.ResultList.Count;
                            }

                          
                        }

                    }
                    Console.WriteLine($"当前获取数据条数：{_usingNavList.Count}");
                }
            }
        }

        private void DealPriceDistcuss(ref string numDistcussCountStr)
        {
            //"\n            人均\n            <b>￥<svgmtsi class=\"shopNum\"></svgmtsi><svgmtsi class=\"shopNum\"></svgmtsi></b>\n            \n         numDistcussCountStr = numDistcussCountStr.Substring(numDistcussCountStr.IndexOf("<b>"));
            numDistcussCountStr = numDistcussCountStr.Replace("人均", "");
            numDistcussCountStr = numDistcussCountStr.Replace("</b>", "");
            numDistcussCountStr = numDistcussCountStr.Replace("<b>", "");
            numDistcussCountStr = numDistcussCountStr.Replace("\n", "").Trim();
            numDistcussCountStr = numDistcussCountStr.Replace("<svgmtsi class=\"shopNum\">", "");
            numDistcussCountStr = numDistcussCountStr.Replace("</svgmtsi>", "");
            numDistcussCountStr = numDistcussCountStr.Replace("￥", "");
            var oneIndex = numDistcussCountStr.IndexOf(Global.special);//数字为1特殊处理
            if (oneIndex >= 0)
            {
                numDistcussCountStr = numDistcussCountStr.Replace("1", "");
            }
            string utf8Str = "";
            for (int i = 0; i < numDistcussCountStr.Length; i++)
            {
                utf8Str += ConvertUtf(numDistcussCountStr.Substring(i, 1));
            }

            if (string.IsNullOrEmpty(utf8Str))
            {
                numDistcussCountStr = "";//异常置空值
                return;
            }
            //todo 匹配字符转成成数字或者合理的中文

            if (oneIndex >= 0)
            {
                numDistcussCountStr = numDistcussCountStr.Insert(oneIndex, Global.special);
            }
        }

        private void DealNumDistcuss(ref string numDistcussCountStr)
        {
            numDistcussCountStr = numDistcussCountStr.Substring(numDistcussCountStr.IndexOf("<b>"));
            numDistcussCountStr = numDistcussCountStr.Replace("条评价", "");
            numDistcussCountStr = numDistcussCountStr.Replace("<svgmtsi class=\"shopNum\">", "");
            numDistcussCountStr = numDistcussCountStr.Replace("</svgmtsi>", "");
            numDistcussCountStr = numDistcussCountStr.Replace("</b>", "");
            numDistcussCountStr = numDistcussCountStr.Replace("<b>", "");
            numDistcussCountStr = numDistcussCountStr.Replace("\n", "");
            var oneIndex = numDistcussCountStr.IndexOf(Global.special);//数字为1特殊处理
            if (oneIndex >= 0)
            {
                numDistcussCountStr = numDistcussCountStr.Replace("1", "");
            }

            string utf8Str = "";
            for (int i = 0; i < numDistcussCountStr.Length; i++)
            {
                utf8Str += ConvertUtf(numDistcussCountStr.Substring(i, 1));
            }

            if (string.IsNullOrEmpty(utf8Str))
            {
                numDistcussCountStr = "";//异常置空值
                return;
            }
            //todo 匹配字符转成成数字或者合理的中文

            if (oneIndex >= 0)
            {
                numDistcussCountStr = numDistcussCountStr.Insert(oneIndex, Global.special);
            }
        }

    
        /// <summary>
        /// 页面乱码字符转UTF-8
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ConvertUtf(string text)
        {
            //汉字转为Unicode编码：
            byte[] b = Encoding.Unicode.GetBytes(text);
            string o = "";
            foreach (var x in b)
            {
                o += string.Format("{0:X2}", x);
            }
            string beginstr = o.Substring(2);
            o = o.Substring(0, 2);
            string result = beginstr + o+";";
            return result;
        }
        /// <summary>
        /// 对Url进行解码,对于javascript的encodeURIComponent函数编码参数,应使用utf-8字符编码来解码
        /// </summary>
        /// <param name="url">url</param>
        public static string UrlDecode(string url)
        {
            return System.Web.HttpUtility.UrlDecode(url, System.Text.Encoding.GetEncoding("GB2312"));
            //return System.Web.HttpUtility.UrlDecode(url, Encoding.UTF8);
        }

        //private void WebBrowserDetail_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        //{

        //    if (e.HttpStatusCode.Equals(200))
        //    {
        //        string resultStr = "";
        //        var task = e.Frame.GetSourceAsync();
        //        task.ContinueWith(t =>
        //        {
        //            if (!t.IsFaulted)
        //            {
        //                resultStr = t.Result;
        //                if (!string.IsNullOrEmpty(resultStr))
        //                {

        //                }
        //            }
        //        });
        //    }
        //}



        /// <summary>
        /// 获取A标签中的 href
        /// </summary>
        /// <param name="AStr"></param>
        /// <returns></returns>
        private string GetHrefByAStr(string AStr)
        {
            string reg = @"<a[^>]*href=([""'])?(?<href>[^'""]+)\1[^>]*>";
            var item = Regex.Match(AStr, reg, RegexOptions.IgnoreCase);
            return item.Groups["href"].Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "所有文件(*.*)|*.*";
            openFileDialog.FileName = "Shop数据.xls";
            openFileDialog.FilterIndex = 1;
            openFileDialog.ValidateNames = false;
            openFileDialog.CheckFileExists = false;
            openFileDialog.CheckPathExists = true;

            openFileDialog.Multiselect = false;//允许同时选择多个文件

            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;//指定启动路径
            var result = openFileDialog.ShowDialog();
            if (result.Equals(DialogResult.Cancel))
            {
                return;
            }
            else
            {
                lock (Global.AccomplisCrawlingUrl)//将爬取完成的url写入到本地文件
                {
                    var fileName = textBox1.Text.Trim() + ".txt";
                    string projectPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                    FileUtilHelper.DeleteFile($"{projectPath}\\searchResule\\{fileName}");
                    projectPath = $"{projectPath}\\searchResule";
                    string tempDirAbsolutePath = Path.Combine(projectPath, fileName);
                    if (!FileUtilHelper.IsExistDirectory(projectPath))
                    {
                        FileUtilHelper.CreateDirectory(projectPath);//创建目录
                    }
                    FileUtilHelper.CreateFile(tempDirAbsolutePath);//创建文件
                    string data = JsonConvert.SerializeObject(Global.AccomplisCrawlingUrl);//转换为字符串
                    object obj = JsonConvert.DeserializeObject<object>(data);//转换为对象
                    using (StreamWriter file = File.CreateText(tempDirAbsolutePath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(file, obj);//将对象存储
                    }
                }
                lock (Global.DateList)
                {
                    string files = openFileDialog.FileNames[0];
                    var excel = new ExcelHelp(files);
                    excel.ExportExcelInfo(files, "sheet1", Global.DateList);
                    MessageBox.Show("数据导出成功");
                }
            }

        }

        /// <summary>
        /// 将检索完成的列表页面全部保存到指定文件中
        /// </summary>
        private void SaveListUrlTxt()
        {
            var fileName = textBox1.Text.Trim() + "_accomplish.txt";
            string projectPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            FileUtilHelper.DeleteFile($"{projectPath}\\searchResule\\{fileName}");
            projectPath = $"{projectPath}\\searchResule";
            string tempDirAbsolutePath = Path.Combine(projectPath, fileName);
            if (!FileUtilHelper.IsExistDirectory(projectPath))
            {
                FileUtilHelper.CreateDirectory(projectPath);//创建目录
            }
            FileUtilHelper.CreateFile(tempDirAbsolutePath);//创建文件
            lock (_usingNavList)
            {
                string data = JsonConvert.SerializeObject(_usingNavList);//转换为字符串
                object obj = JsonConvert.DeserializeObject<object>(data);//转换为对象
                using (StreamWriter file = File.CreateText(tempDirAbsolutePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, obj);//将对象存储
                }
            }
        }

        /// <summary>
        /// 清除所有历史爬取记录的url
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            string projectPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            projectPath = Path.Combine(projectPath, "searchResule");
            if (FileUtilHelper.IsExistDirectory(projectPath))
            {
                string[] fileNamePaths = FileUtilHelper.GetFileNames(projectPath, "*.txt", false);
                if (fileNamePaths.Length > 0)
                {
                    for (int i = 0; i < fileNamePaths.Length; i++)
                    {
                        FileUtilHelper.DeleteFile(fileNamePaths[i]);
                    }
                }
            }

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.Focus();
            richTextBox1.Select(richTextBox1.Text.Length, 0);
            richTextBox1.ScrollToCaret();
        }

        /// <summary>
        /// 暂停爬取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            this.isSuspend = true;
        }

    }
}
