using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArchitectureDesignSafety;
using CefSharp;
using CefSharp.WinForms;

namespace AlbbData
{
    public partial class SclimbForm : Form
    {
          ChromiumWebBrowser _webview;
        public SclimbForm(string url)
        {
            InitializeComponent();
             this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            _webview = CefProxyOptions.Create(url);
            _webview.FrameLoadEnd += Webview_FrameLoadEnd;
            _webview.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(_webview);
        }

        private void Webview_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
              _webview = (ChromiumWebBrowser)sender;
            if (e.HttpStatusCode.Equals(200))
            {
                  string resultStr = "";
                    var task02 = e.Frame.GetSourceAsync();
                    task02.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            resultStr = t.Result;
                            if (!string.IsNullOrEmpty(resultStr))
                            {
                                InsertListObj(resultStr);//插入数据
                            }
                        }
                    });

            }
        }

        private void InsertListObj(string resultStr)
        {
             
        }
    }
}
