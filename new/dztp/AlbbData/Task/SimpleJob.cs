using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchitectureDesignSafety;
using CefSharp;
using Quartz;

namespace AlbbData
{
    public class SimpleJob : IJob
    {
        public virtual System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            //return Console.Out.WriteLineAsync($"job工作了 在{DateTime.Now}");
           var  _webview = CefProxyOptions.Create(Global.FirstUrl);
            _webview.FrameLoadEnd += Webview_FrameLoadEnd;
            return Console.Out.WriteLineAsync($"job工作了 在{DateTime.Now}");
        }

        private void Webview_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            
            if (e.HttpStatusCode.Equals(200))
            {
               
            }
        }
    }
}
