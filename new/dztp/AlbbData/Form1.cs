using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using AlbbData.Tool;

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
        private List<string> _usingNavList = new List<string>();

        private string _searchPinying = "";

        private List<ListUrlEntity> _listNav;
        private string _thisSearchTypeName;
        private List<string> _listPageHtmlContent = new List<string>();
        private SqlSugarClientHelp _sqlSugarClient;


        private int _listSheepTime = 2000;
        private int _detailSheepTime = 3500;

        private bool _isStopDetailPaqu = false;//是否暂停详细页爬取


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
            textBox1.Text = "烟台";
            #region 创建发布
            provider = new BaggageHandler();
            if (!Global.providers.Exists(f => f._id == provider._id))
                Global.providers.Add(provider);
            #endregion


            #region 注册订阅者，并注册写入数据事件
            observer = new ArrivalsMonitor("BaggageClaimMonitor1");
            observer.OnChanged += Observer_OnChanged;

            #endregion

            DZDPCode.ReadAllTExtCodeInfo();
            if (_sqlSugarClient == null)
            {
                _sqlSugarClient = new SqlSugarClientHelp();
            }
        }

        /// <summary>
        /// //构造检索树
        /// </summary>
        private void InitListUrlInfo()
        {
            _listNav = new List<ListUrlEntity>();
            ListUrlEntity temp;
            #region 美食

            //ListUrlEntity temp = new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch10",
            //    Name = "美食",
            //    isAccomplishSearchingList = true,
            //    ChildrenEntityList = new List<ListUrlEntity>()
            //};
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch10/g110",
            //    isAccomplishSearchingList = false,
            //    Name = "火锅",
            //});
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch10/g117",
            //    isAccomplishSearchingList = false,
            //    Name = "面包甜点",
            //});
            //_listNav.Add(temp);

            #endregion

            #region 休闲娱乐
            //ListUrlEntity temp = new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch30",
            //    Name = "休闲娱乐",
            //    isAccomplishSearchingList = true,
            //    ChildrenEntityList = new List<ListUrlEntity>()
            //};
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch30/g141",
            //    isAccomplishSearchingList = false,
            //    Name = "足疗",
            //});
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch30/g135",
            //    isAccomplishSearchingList = false,
            //    Name = "KTV",
            //});
            //_listNav.Add(temp);
            
            #endregion

            #region 结婚
            temp = new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholder/wedding",
                Name = "结婚",
                isAccomplishSearchingList = true,
                ChildrenEntityList = new List<ListUrlEntity>()
            };
            temp.ChildrenEntityList.Add(new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch55/g163",
                isAccomplishSearchingList = true,
                Name = "婚纱摄影",
            });
            temp.ChildrenEntityList.Add(new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch55/g167",
                isAccomplishSearchingList = true,
                Name = "婚礼策划",
            });
            _listNav.Add(temp);
            #endregion


            #region 电影演出赛事
            //temp = new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/movie",
            //    Name = "电影演出赛事",
            //    isAccomplishSearchingList = true,
            //    ChildrenEntityList = new List<ListUrlEntity>()
            //};
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch25/g136",
            //    isAccomplishSearchingList = false,
            //    Name = "电影院",
            //});
            //_listNav.Add(temp);
            #endregion

            #region 丽人
            //temp = new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/beauty",
            //    Name = "丽人",
            //    isAccomplishSearchingList = true,
            //    ChildrenEntityList = new List<ListUrlEntity>()
            //};
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch50/g157",
            //    isAccomplishSearchingList = false,
            //    Name = "美发",
            //});
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch50/g183",
            //    isAccomplishSearchingList = false,
            //    Name = "医学美容",
            //});
            //_listNav.Add(temp);
            #endregion

            #region 酒店
            temp = new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholder/hotel",
                Name = "酒店",
                isAccomplishSearchingList = true,
                ChildrenEntityList = new List<ListUrlEntity>()
            };
            temp.ChildrenEntityList.Add(new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholder/hotel/g3020",
                isAccomplishSearchingList = true,
                Name = "五星/豪华",
            });
            temp.ChildrenEntityList.Add(new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholder/hotel/g171",
                isAccomplishSearchingList = true,
                Name = "经济连锁",
            });
            _listNav.Add(temp);
            #endregion

            #region 酒店
            temp = new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholder/hotel",
                Name = "酒店",
                isAccomplishSearchingList = true,
                ChildrenEntityList = new List<ListUrlEntity>()
            };
            temp.ChildrenEntityList.Add(new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholder/hotel/g3020",
                isAccomplishSearchingList = true,
                Name = "五星/豪华",
            });
            temp.ChildrenEntityList.Add(new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholder/hotel/g171",
                isAccomplishSearchingList = true,
                Name = "经济连锁",
            });
            _listNav.Add(temp);
            #endregion

            #region 亲子
            temp = new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholder/baby",
                Name = "亲子",
                isAccomplishSearchingList = true,
                ChildrenEntityList = new List<ListUrlEntity>()
            };
            temp.ChildrenEntityList.Add(new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch70/g193",
                isAccomplishSearchingList = true,
                Name = "亲子摄影",
            });
            temp.ChildrenEntityList.Add(new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch75/g34309",
                isAccomplishSearchingList = true,
                Name = "早教中心",
            });
            _listNav.Add(temp);
            #endregion

            #region 周边游
            //temp = new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch35",
            //    Name = "周边游",
            //    isAccomplishSearchingList = true,
            //    ChildrenEntityList = new List<ListUrlEntity>()
            //};
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch35/g33831",
            //    isAccomplishSearchingList = true,
            //    Name = "景点",
            //});
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch35/g2916",
            //    isAccomplishSearchingList = false,
            //    Name = "水上娱乐",
            //});
            //_listNav.Add(temp);
            #endregion

            #region 运动健身
            //temp = new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch45",
            //    Name = "运动健身",
            //    isAccomplishSearchingList = true,
            //    ChildrenEntityList = new List<ListUrlEntity>()
            //};
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch45/g147",
            //    isAccomplishSearchingList = false,
            //    Name = "健身中心",
            //});
            //_listNav.Add(temp);
            #endregion

            #region 购物
            //temp = new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch20",
            //    Name = "购物",
            //    isAccomplishSearchingList = true,
            //    ChildrenEntityList = new List<ListUrlEntity>()
            //};
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch20/g119",
            //    isAccomplishSearchingList = false,
            //    Name = "综合商场",
            //});
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch20/g120",
            //    isAccomplishSearchingList = false,
            //    Name = "服饰鞋包",
            //});
            //_listNav.Add(temp);
            #endregion

            #region 家装
            //temp = new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/home",
            //    Name = "家装",
            //    isAccomplishSearchingList = true,
            //    ChildrenEntityList = new List<ListUrlEntity>()
            //};
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch90/g25475",
            //    isAccomplishSearchingList = true,
            //    Name = "装修设计",
            //});
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch90/g34035",
            //    isAccomplishSearchingList = true,
            //    Name = "定制家居",
            //});
            //_listNav.Add(temp);
            #endregion

            #region 学习培训
            //temp = new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/education",
            //    Name = "学习培训",
            //    isAccomplishSearchingList = true,
            //    ChildrenEntityList = new List<ListUrlEntity>()
            //};
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch75/g2872",
            //    isAccomplishSearchingList = false,
            //    Name = "外语",
            //});
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch75/g34319",
            //    isAccomplishSearchingList = false,
            //    Name = "音乐",
            //});
            //_listNav.Add(temp);
            #endregion

            #region 生活服务
            //temp = new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholde/ch80",
            //    Name = "生活服务",
            //    isAccomplishSearchingList = true,
            //    ChildrenEntityList = new List<ListUrlEntity>()
            //};
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch80/g3064",
            //    isAccomplishSearchingList = false,
            //    Name = "快照",
            //});
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch80/g195",
            //    isAccomplishSearchingList = false,
            //    Name = "家政",
            //});
            //_listNav.Add(temp);
            #endregion



            #region 医疗健康
            //temp = new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholde/ch85",
            //    Name = "医疗健康",
            //    isAccomplishSearchingList = true,
            //    ChildrenEntityList = new List<ListUrlEntity>()
            //};
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch85/g182",
            //    isAccomplishSearchingList = false,
            //    Name = "齿科",
            //});
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch85/g612",
            //    isAccomplishSearchingList = false,
            //    Name = "体检中心",
            //});
            //_listNav.Add(temp);
            #endregion

            #region 爱车
            //temp = new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholde/ch65",
            //    Name = "爱车",
            //    isAccomplishSearchingList = true,
            //    ChildrenEntityList = new List<ListUrlEntity>()
            //};
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch65/g34072",
            //    isAccomplishSearchingList = false,
            //    Name = "美容洗车",
            //});
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch65/g176",
            //    isAccomplishSearchingList = false,
            //    Name = "维修保养",
            //});
            //_listNav.Add(temp);
            #endregion


            #region 宠物
            temp = new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholde/ch95",
                Name = "宠物",
                isAccomplishSearchingList = true,
                ChildrenEntityList = new List<ListUrlEntity>()
            };
            //temp.ChildrenEntityList.Add(new ListUrlEntity()
            //{
            //    UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch95/g25147",
            //    isAccomplishSearchingList = false,
            //    Name = "宠物店",
            //});
            temp.ChildrenEntityList.Add(new ListUrlEntity()
            {
                UrlAddress = "http://www.dianping.com/Cite_Placeholder/ch95/g25148",
                isAccomplishSearchingList = false,
                Name = "宠物医院",
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
            InitListUrlInfo();//构造检索树
            //InitListCralIngUrl();//初始化 构造历史爬取 列表也的url数据
            //InitAccomplisCrawlingUrl();//初始化 构造历史爬取完成的url数据
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请输入要检索的城市");
            }
            var navInfo = SetThisTypeAndWriteTypeName();
            if (navInfo != null)
            {
                _searchPinying = PingYinHelper.ConvertToAllSpell(textBox1.Text.Trim()).ToLower();
                navInfo.UrlAddress = navInfo.UrlAddress.Replace("Cite_Placeholder", _searchPinying);//替换城市占位符
            }
            _isExecuteSearch = false;
            if (_webview != null && !_webview.IsDisposed)
            {
                _webview.Load(navInfo.UrlAddress);
            }
            else
            {
                _webview = CefProxyOptions.CreateSingleCookie(navInfo.UrlAddress, "userInfoCookie");
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
        private ListUrlEntity SetThisTypeAndWriteTypeName(int beginIndex = 0)
        {
            for (int i = beginIndex; i < _listNav.Count; i++)
            {
                if (!_listNav[i].isAccomplishSearchingList)
                {
                    _listNav[i].isAccomplishSearchingList = true;
                    _thisSearchTypeName = _listNav[i].Name;
                    return _listNav[i];
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
                                _thisSearchTypeName = _listNav[i].ChildrenEntityList[j].Name;
                                return _listNav[i].ChildrenEntityList[j];
                            }
                        }
                        return SetThisTypeAndWriteTypeName(i + 1);
                    }
                }
            }
            return null;
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
                        await Task.Delay(500);//页面加载完成时间
                        ScollHeightBottomAndAsyncGetDataAndJumpNextPage();
                    });
                    _initListPageNav.Number++;
                }
                else
                {
                    GetData();
                    _initListPageNav.Number = 0;
                }
            }

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
                            lock (_listPageHtmlContent)
                            {
                                if (getIsExitResult(resultStr, _listPageHtmlContent))
                                    return;
                                else
                                {
                                    _listPageHtmlContent.Add(resultStr);
                                    if (_listPageHtmlContent.Count > 100)
                                    {
                                        _listPageHtmlContent.RemoveRange(0, 70);
                                    }
                                }
                            }
                            bool isInsertSuccess = GetThisPageAllListDataAndGetDetialData(resultStr);//返回是否插入数据成功
                            if (!isInsertSuccess)//如果插入数据位0条，就直接退出
                            {
                                return;
                            }

                            ToNextPageJs(); //to next page

                        }
                    }
                });
            }
        }

        /// <summary>
        /// 判断历史是否检索过此内容
        /// </summary>
        /// <param name="resultStr">即将要检索的内容</param>
        /// <param name="listPageHtmlContent">历史检索过的内容</param>
        /// <returns></returns>
        private bool getIsExitResult(string resultStr, List<string> listPageHtmlContent)
        {
            bool isexit = false;
            if (_listPageHtmlContent.Contains(resultStr))
            {
                isexit = true;
            }
            else
            {
                _listPageHtmlContent.ForEach(f =>
                {
                    if (resultStr.Contains(f))
                    {
                        isexit = true;
                    }
                });
            }
            return isexit;
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

        /// <summary>
        /// 继续爬取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            this._isStopDetailPaqu = false;
            if (Global.ResultList != null && Global.ResultList.Count > 0)
            {
                var navInfo = GetNext1NavByGlobalData();
                if (navInfo != null)
                {
                    //Global.ResultList.RemoveAt(0);
                    Console.WriteLine($"开始导航到---》{navInfo.Name} ==>>{navInfo.DetailNavUrl}");
                    CreateDetailPageWebView(navInfo);
                }
                else
                {
                    Console.WriteLine("检索完成全部详细页面");
                }
            }
        }

        private void ToDetailPageGetPhoneAndAddress()
        {
            if (Global.ResultList != null && Global.ResultList.Count > 0)
            {
                var NavInfo = GetNextNavByGlobalData();
                if (NavInfo != null)
                {
                    //Global.ResultList.RemoveAt(0);
                    Console.WriteLine($"开始导航到---》{NavInfo.Name} ==>>{NavInfo.DetailNavUrl}");
                    CreateDetailPageWebView(NavInfo);
                }
                else
                {
                    Console.WriteLine("检索完成全部详细页面");
                }
            }
        }

        private ShopListInfoEntity GetNext1NavByGlobalData()
        {
            bool isFirst = false;
            if (Global.ResultList != null && Global.ResultList.Count > 0)
            {
                for (var i = 0; i < Global.ResultList.Count; i++)
                {
                    if (!Global.ResultList[i].isAccomplishNavDetail && !isFirst)
                    {
                        Global.ResultList[i].isAccomplishNavDetail = true;
                        isFirst = true;
                    }
                    else if (!Global.ResultList[i].isAccomplishNavDetail)
                    {
                        ShopListInfoEntity thisResult = Global.ResultList[i];
                        return thisResult;
                    }
                }
            }
            return null;
        }

        private ShopListInfoEntity GetNextNavByGlobalData()
        {
            if (Global.ResultList != null && Global.ResultList.Count > 0)
            {
                for (var i = 0; i < Global.ResultList.Count; i++)
                {
                    if (!Global.ResultList[i].isAccomplishNavDetail)
                    {
                        ShopListInfoEntity thisResult = Global.ResultList[i];
                        return thisResult;
                    }
                }
            }
            return null;
        }






        public void CreateDetailPageWebView(ShopListInfoEntity shopInfoEntity)
        {
            if (_isStopDetailPaqu)
                return;
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { CreateDetailPageWebView(shopInfoEntity); }));
                    return;
                }

                if (_webview != null && !_webview.IsDisposed)
                {
                    _webview.Load(shopInfoEntity.DetailNavUrl);
                    _webview.FrameLoadEnd += DetailWebview_FrameLoadEnd;
                    panel1.Controls.Clear();
                    panel1.Controls.Add(_webview);
                }
                else
                {
                    var detailWebView = CefProxyOptions.Create(shopInfoEntity.DetailNavUrl);
                    detailWebView.FrameLoadEnd += DetailWebview_FrameLoadEnd;
                    panel1.Controls.Clear();
                    panel1.Controls.Add(detailWebView);

                }


                //var task = Task.Run(async delegate
                //{
                //    lock (_taskNav)
                //    {
                //        if (_taskNav.Number >= 0)
                //        {
                //            DealDetailAndGetDate();
                //            _taskNav.Number--;
                //        }
                //    }
                //});
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void DetailWebview_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            try
            {
                var detialWebview = (ChromiumWebBrowser)sender;
                if (e.HttpStatusCode.Equals(200))
                {
                    var thisUrl = e.Url;
                    InsertListObj(e, detialWebview, thisUrl);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }



        private void InsertListObj(FrameLoadEndEventArgs e, ChromiumWebBrowser detialWebview, string thisUrl)
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
                                    DateInsert(resultStr, detialWebview, thisUrl);
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

        private void DateInsert(string resultStr, ChromiumWebBrowser detialWebview, string url)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(resultStr);
            var phoneSpanNode = doc.DocumentNode.SelectSingleNode("//p[@class='expand-info tel']");// 电话
            var addressNode = doc.DocumentNode.SelectSingleNode("//div[@class='expand-info address']");//地址
            var districtNode = doc.DocumentNode.SelectSingleNode("//div[@class='breadcrumb']");//区域
            var commentNode = doc.DocumentNode.SelectSingleNode("//span[@id='reviewCount']");//评论
            var priceNode = doc.DocumentNode.SelectSingleNode("//span[@id='avgPriceTitle']");//人均
            var midScoreNode = doc.DocumentNode.SelectSingleNode("//div[@id='mid-score']");//评分
            if (phoneSpanNode != null || addressNode != null || districtNode != null || priceNode != null || commentNode != null|| midScoreNode!=null)
            {
                lock (Global.ResultList)
                {
                    var index = Global.ResultList.FindIndex(f => f.DetailNavUrl == url);
                    if (index < 0)
                        return;
                    var thisShopListInfo = Global.ResultList[index];
                    var thisShopInfo = new ShopInfoEntity();
                    if (thisShopListInfo != null)
                    {
                        thisShopInfo.Name = thisShopListInfo.Name;
                        thisShopInfo.DetailNavUrl = thisShopListInfo.DetailNavUrl;
                        thisShopInfo.ShopType = thisShopListInfo.ShopType;
                        thisShopInfo.City = thisShopListInfo.City;
                    }

                    //var commentChildrenNode = commentNode!=null? commentNode.ChildNodes:null;
                    if (commentNode != null)
                    {
                        var commentStr = commentNode.InnerText;
                        DealNumDistcuss(ref commentStr);
                        thisShopInfo.DiscussCount = commentStr;
                    }
                    else
                    {

                    }

                    //var priceChildrenNode = priceNode != null ? priceNode.ChildNodes : null;
                    if (priceNode != null)
                    {
                        var priceStr = priceNode.InnerText;
                        DealPriceDistcuss(ref priceStr);
                        thisShopInfo.CustomPrice = priceStr;
                    }
                    var phoneChildrens = phoneSpanNode.ChildNodes;
                    if (phoneChildrens.Count > 5)
                    {
                        for (int i = 1; i < phoneChildrens.Count; i++)
                        {
                            thisShopInfo.Phone += phoneChildrens[i].InnerHtml;
                        }

                        if (thisShopInfo.Phone != null && !string.IsNullOrEmpty(thisShopInfo.Phone))
                        {
                            thisShopInfo.Phone = thisShopInfo.Phone.Trim();
                            string phone = thisShopInfo.Phone;
                            DealNumDistcuss(ref phone);
                            thisShopInfo.Phone = phone;
                        }
                    }
                    else
                    {

                    }
                    if (addressNode != null)
                    {
                        if (addressNode.ChildNodes != null && addressNode.ChildNodes.Count > 3)
                        {
                            thisShopInfo.Address = addressNode.ChildNodes[3].InnerHtml;
                            string address = thisShopInfo.Address;
                            DealAddressDistcuss(ref address);
                            thisShopInfo.Address = address;
                        }
                        else
                        {

                        }
                    }
                    if (districtNode != null)
                    {
                        var districtStr = districtNode.InnerText;
                        if (!string.IsNullOrEmpty(districtStr))
                        {
                            bool isContainDisList = false;
                            var disList = Regex.Split(districtStr, "&gt;", RegexOptions.IgnoreCase).ToList();
                            if (disList.Count > 0)
                            {
                                for (int i = 0; i < disList.Count; i++)
                                {
                                    disList[i] = disList[i].Trim();
                                    if (disList[i].LastIndexOf("区") >= 0)
                                    {
                                        thisShopInfo.District = disList[i];
                                        isContainDisList = true;
                                    }
                                }
                            }
                            if (!isContainDisList)
                            {
                                thisShopInfo.District = districtStr.Replace("&gt;", "").Trim();
                            }
                        }
                    }
                    else
                    {

                    }

                    if (midScoreNode != null)
                    {
                        var midScorStr = midScoreNode.InnerText;
                        thisShopInfo.ShopScore = midScorStr;
                    }
                    else
                    {

                    }

                    thisShopInfo.isAccomplishNavDetail = true;
                    ModifListObj(thisShopListInfo,index);
                    InsertDataBaseInfo(thisShopInfo);

                    var t = Task.Run(async delegate
                    {
                        await Task.Delay(_detailSheepTime);//页面加载完成时间，看网速，这里后期可能需要调整
                            ToDetailPageGetPhoneAndAddress();//跳转下一条检索内容
                    });
                }


            }


        }

        /// <summary>
        /// 保存list数据，并修改全局变量中的list对象为已经检索
        /// </summary>
        /// <param name="thisShopListInfo"></param>
        /// <param name="index"></param>
        private void ModifListObj(ShopListInfoEntity thisShopListInfo,int index)
        {
            thisShopListInfo.isAccomplishNavDetail = true;
            Global.ResultList.RemoveAt(index);
            Global.ResultList.Insert(index, thisShopListInfo);
            var showListObj = _sqlSugarClient.ShopListInfoDb.GetSingle(f =>
                f.Name == thisShopListInfo.Name && f.DetailNavUrl == thisShopListInfo.DetailNavUrl);
            if (showListObj != null)
            {
                showListObj.isAccomplishNavDetail = true;
                _sqlSugarClient.ShopListInfoDb.Update(showListObj);
            }
        }

        /// <summary>
        ///数据库插入数据
        /// </summary>
        /// <param name="thisShopInfo"></param>
        private void InsertDataBaseInfo(ShopInfoEntity thisShopInfo)
        {
            try
            {
                if (_sqlSugarClient.ShopInfoDb.GetSingle(f =>
                    f.Name == thisShopInfo.Name && f.DetailNavUrl == thisShopInfo.DetailNavUrl) == null)
                {
                    _sqlSugarClient.ShopInfoDb.Insert(thisShopInfo);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private void ToNextPageJs()
        {

            var t = Task.Run(async delegate
                {
                    await Task.Delay(_listSheepTime);//页面加载完成时间，看网速，这里后期可能需要调整
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
            //InitAccomplisCrawlingUrl();//初始化 构造历史爬取完成的url数据
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请输入要检索的城市");
            }
            var navInfo = SetThisTypeAndWriteTypeName();
            if (navInfo != null)
            {
                _searchPinying = PingYinHelper.ConvertToAllSpell(textBox1.Text.Trim());
                navInfo.UrlAddress = navInfo.UrlAddress.Replace("Cite_Placeholder", _searchPinying);
            }
            if (navInfo != null)//还有列表目录没有检索完成
            {
                _webview.Load(navInfo.UrlAddress);
                _webview.FrameLoadEnd += Webview_FrameLoadEnd;
            }
            else//全部列表目录检索完成，可以检索详细页了
            {
                Console.WriteLine("所有的列表页检索完成");
                //MessageBox.Show("所有的列表页检索完成");
                //ToDetailPageGetPhoneAndAddress();
            }
        }



        /// <summary>
        /// 整合列表数据
        /// </summary>
        /// <param name="resultStr"></param>
        private bool GetThisPageAllListDataAndGetDetialData(string resultStr)
        {
            bool IsInsertSuccess = false;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(resultStr);
            var htmlNode = doc.DocumentNode.SelectSingleNode("//div[@class='shop-list J_shop-list shop-all-list']");
            if (htmlNode != null)
            {
                HtmlAgilityPack.HtmlNodeCollection nodecollection = htmlNode.ChildNodes[1].ChildNodes;
                if (nodecollection.Count > 15)
                {
                    var htmlCity = doc.DocumentNode.SelectSingleNode("//span[@itemprop='title']");
                    for (int i = 0; i < nodecollection.Count; i++)
                    {
                        if (nodecollection[i].ChildNodes.Count > 0)
                        {
                            string cityStr = "";
                            if (htmlCity != null)
                            {
                                cityStr = htmlCity.InnerText.Replace("\n", "").Trim();
                                cityStr = cityStr.Replace(_thisSearchTypeName, "");
                            }

                            ShopListInfoEntity tempshwoinfo = new ShopListInfoEntity
                            {
                                Name = nodecollection[i].ChildNodes[3].ChildNodes[1].ChildNodes[1].ChildNodes[1].InnerText,
                                DetailNavUrl = GetHrefByAStr(nodecollection[i].ChildNodes[3].ChildNodes[1].ChildNodes[1].OuterHtml),
                                City = cityStr,
                                ShopType = _thisSearchTypeName,
                                CreateDateTime = DateTime.Now,
                                isAccomplishNavDetail = false,
                            };
                            if (!Global.ResultList.Exists(f =>
                                f.Name == tempshwoinfo.Name && f.DetailNavUrl == tempshwoinfo.DetailNavUrl))
                            {
                                Global.ResultList.Add(tempshwoinfo);
                                IsInsertSuccess = true;
                            }

                            InsertListDataToDataBase(tempshwoinfo);//将list页检索完成的实体存储到数据库中
                        }

                    }
                    Console.WriteLine($"当前获取数据条数：{Global.ResultList.Count}");
                }
            }
            return IsInsertSuccess;
        }

        /// <summary>
        /// 将list页检索完成的实体存储到数据库中
        /// </summary>
        /// <param name="tempshwoinfo"></param>
        private void InsertListDataToDataBase(ShopListInfoEntity tempshwoinfo)
        {
            if (_sqlSugarClient.ShopListInfoDb.GetSingle(f =>
                f.Name == tempshwoinfo.Name && f.DetailNavUrl == tempshwoinfo.DetailNavUrl) == null)
            {
                _sqlSugarClient.ShopListInfoDb.Insert(tempshwoinfo);
            }
        }

        private void DealAddressDistcuss(ref string numDistcussCountStr)
        {
            try
            {
                numDistcussCountStr = numDistcussCountStr.Replace("<span class=\"item\" itemprop=\"street-address\" id=\"address\">", "");
                numDistcussCountStr = numDistcussCountStr.Replace("</span> <div class=\"addressIcon\"></div>", "");
                numDistcussCountStr = numDistcussCountStr.Replace("<e class=\"address\">", "");
                numDistcussCountStr = numDistcussCountStr.Replace("</e>", "");
                numDistcussCountStr = numDistcussCountStr.Replace("<d class=\"num\">", "");
                numDistcussCountStr = numDistcussCountStr.Replace("</d>", "");
                numDistcussCountStr = numDistcussCountStr.Trim();
                List<ChinaInfo> ChinaInfoList = GetAllChinaInfos(numDistcussCountStr);//找出所有没有加密的中文字符
                if (ChinaInfoList.Count > 0)//删除没有加密的中文字符
                {
                    for (int i = ChinaInfoList.Count - 1; i >= 0; i--)
                    {
                        numDistcussCountStr = numDistcussCountStr.Replace(ChinaInfoList[i].China, "");
                    }
                }

                string utf8Str = "";

                for (int i = 0; i < numDistcussCountStr.Length; i++)
                {
                    utf8Str += DZDPCode.MatchingCode(ConvertUtf(numDistcussCountStr.Substring(i, 1)));
                }

                if (string.IsNullOrEmpty(utf8Str))
                {
                    numDistcussCountStr = "";//异常置空值
                    return;
                }
                if (ChinaInfoList.Count > 0)//将删除的中文字符恢复插入
                {
                    for (int i = 0; i < ChinaInfoList.Count; i++)
                    {
                        utf8Str = utf8Str.Insert(ChinaInfoList[i].Index, ChinaInfoList[i].China);
                    }
                }
                numDistcussCountStr = utf8Str;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public List<ChinaInfo> GetAllChinaInfos(string str)
        {
            List<ChinaInfo> resultChinaInfos = new List<ChinaInfo>();
            for (int counter = 0; counter < str.Length; counter++)//中文字符
            {
                if (str[counter] >= 0x4E00 && str[counter] <= 0x9FA5)
                {
                    var tempInfo = new ChinaInfo
                    {
                        Index = counter,
                        China = str[counter].ToString()
                    };
                    resultChinaInfos.Add(tempInfo);
                }
            }

            for (int counter = 0; counter < str.Length; counter++)//特殊字符 1
            {
                if (str[counter].ToString().Equals(Global.special))
                {
                    var tempInfo = new ChinaInfo
                    {
                        Index = counter,
                        China = Global.special
                    };
                    resultChinaInfos.Add(tempInfo);
                }
            }
            return resultChinaInfos;
        }


        private void DealPriceDistcuss(ref string numDistcussCountStr)
        {
            try
            {
                numDistcussCountStr = numDistcussCountStr.Replace("人均", "");
                numDistcussCountStr = numDistcussCountStr.Replace("：", "");
                numDistcussCountStr = numDistcussCountStr.Replace("元", "");
                numDistcussCountStr = numDistcussCountStr.Replace("</b>", "");
                numDistcussCountStr = numDistcussCountStr.Replace("<b>", "");
                numDistcussCountStr = numDistcussCountStr.Replace("\n", "").Trim();
                numDistcussCountStr = numDistcussCountStr.Replace("<svgmtsi class=\"shopNum\">", "");
                numDistcussCountStr = numDistcussCountStr.Replace("</svgmtsi>", "");
                numDistcussCountStr = numDistcussCountStr.Replace("￥", "");
                numDistcussCountStr = numDistcussCountStr.Trim();
                List<int> speciallIndexs = GetAllAppearIndexs(numDistcussCountStr);//判断特殊字符出现个次数
                if (speciallIndexs != null && speciallIndexs.Count >= 0)
                {
                    numDistcussCountStr = numDistcussCountStr.Replace(Global.special, "");
                }
                string utf8Str = "";
                for (int i = 0; i < numDistcussCountStr.Length; i++)
                {
                    utf8Str += DZDPCode.MatchingCode(ConvertUtf(numDistcussCountStr.Substring(i, 1)));
                }

                if (string.IsNullOrEmpty(utf8Str))
                {
                    numDistcussCountStr = "";//异常置空值
                    return;
                }

                if (speciallIndexs != null && speciallIndexs.Count >= 0)
                {
                    for (int i = 0; i < speciallIndexs.Count; i++)
                    {
                        utf8Str = utf8Str.Insert(speciallIndexs[i], Global.special);
                    }
                }

                numDistcussCountStr = utf8Str;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private void DealNumDistcuss(ref string numDistcussCountStr)
        {
            try
            {
                var index = numDistcussCountStr.IndexOf("<b>");
                if (index >= 0)
                {
                    numDistcussCountStr = numDistcussCountStr.Substring(index);
                }
                numDistcussCountStr = numDistcussCountStr.Replace("条评价", "");
                numDistcussCountStr = numDistcussCountStr.Replace("<svgmtsi class=\"shopNum\">", "");
                numDistcussCountStr = numDistcussCountStr.Replace("</svgmtsi>", "");
                numDistcussCountStr = numDistcussCountStr.Replace("</b>", "");
                numDistcussCountStr = numDistcussCountStr.Replace("<b>", "");
                numDistcussCountStr = numDistcussCountStr.Replace("<d class=\"num\">", "");
                numDistcussCountStr = numDistcussCountStr.Replace("</d>", "");
                numDistcussCountStr = numDistcussCountStr.Replace("\n", "");
                numDistcussCountStr = numDistcussCountStr.Replace("&nbsp;", "");
                numDistcussCountStr = numDistcussCountStr.Replace("*", "");
                numDistcussCountStr = numDistcussCountStr.Trim();
                //var oneIndex = numDistcussCountStr.IndexOf(Global.special);//数字为1特殊处理
                List<int> speciallIndexs = GetAllAppearIndexs(numDistcussCountStr);//判断特殊字符出现个次数
                if (speciallIndexs != null && speciallIndexs.Count > 0)
                {
                    numDistcussCountStr = numDistcussCountStr.Replace(Global.special, "");
                }

                string utf8Str = "";
                for (int i = 0; i < numDistcussCountStr.Length; i++)
                {
                    utf8Str += DZDPCode.MatchingCode(ConvertUtf(numDistcussCountStr.Substring(i, 1)));
                }

                if (string.IsNullOrEmpty(utf8Str))
                {
                    numDistcussCountStr = "";//异常置空值
                    return;
                }

                if (speciallIndexs != null && speciallIndexs.Count > 0)
                {
                    for (int i = 0; i < speciallIndexs.Count; i++)
                    {
                        utf8Str = utf8Str.Insert(speciallIndexs[i], Global.special);
                    }
                }

                numDistcussCountStr = utf8Str;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private List<int> GetAllAppearIndexs(string numDistcussCountStr)
        {
            List<int> result = new List<int>();
            for (int counter = 0; counter < numDistcussCountStr.Length; counter++)
            {
                if (numDistcussCountStr[counter].ToString().Equals(Global.special))
                {
                    result.Add(counter);
                }
            }
            return result;
        }

        ///// <summary>
        ///// 数字解码
        ///// </summary>
        ///// <param name="numDistcussCountStr"></param>
        //private void NumberDealDecode(ref string numDistcussCountStr)
        //{

        //}


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
            string result = beginstr + o + ";";
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
            this._isStopDetailPaqu = true;

        }

    }
}
