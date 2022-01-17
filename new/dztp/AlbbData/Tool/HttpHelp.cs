using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp.Tool
{
    public class HttpHelp
    {
        /// <summary>
        /// Http post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string Post(string url, Hashtable postData)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                if (webRequest == null)
                {
                    return "Network error" + new ArgumentNullException("httpWebRequest").Message;
                }
                webRequest.Method = "POST";
                webRequest.KeepAlive = true;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Timeout = 300000;
                webRequest.AllowAutoRedirect = true;
                if (!(postData == null) || postData.Count == 0)
                {
                    StringBuilder buffer = new StringBuilder();
                    bool first = true;
                    foreach (string key in postData.Keys)
                    {
                        if (!first)
                        {
                            buffer.AppendFormat("&{0}={1}", key, postData[key]);
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, postData[key]);
                            first = false;
                        }
                    }
                    byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                    using (Stream stream = webRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                using (Stream s = (webRequest.GetResponse() as HttpWebResponse).GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(s, Encoding.UTF8);
                    string responseContent = streamReader.ReadToEnd();
                    streamReader.Close();
                    return responseContent;
                }
            }
            catch (System.Exception ex)
            {
                return "网络错误(Network error)：" + ex.Message;
            }

        }
        /// <summary>
        /// Http get
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Get(string url)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                if (webRequest == null)
                {
                    return "Network error" + new ArgumentNullException("httpWebRequest").Message;
                }
                webRequest.Method = "GET";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Timeout = 300000;
                webRequest.AllowWriteStreamBuffering = false;
                webRequest.ServicePoint.Expect100Continue = false;
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                using (Stream webstream = webResponse.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(webstream, Encoding.UTF8);
                    string responseContent = streamReader.ReadToEnd();
                    streamReader.Close();
                    return responseContent;
                }
            }
            catch (Exception ex)
            {
                return "网络错误(Network error)：" + ex.Message;
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">下载路径</param>
        /// <param name="processs">下载进度</param>
        /// <returns></returns>
        public static byte[] DownLoadFiles(string url, ref float process)
        {
            byte[] buffer;
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                webRequest.Method = "GET";
                webRequest.Timeout = 10000;
                using (WebResponse response = webRequest.GetResponse())
                {
                    long lengths = response.ContentLength;
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    int length = (int)response.ContentLength;
                    HttpWebResponse webResponse = response as HttpWebResponse;
                    Stream stream = webResponse.GetResponseStream();
                    MemoryStream memory = new MemoryStream();
                    byte[] buffer1 = new byte[1024];
                    long GetLength = 0;
                    int i = stream.Read(buffer1, 0, (int)buffer1.Length);
                    while (i > 0)
                    {
                        GetLength = i + GetLength;
                        memory.Write(buffer1, 0, i);
                        i = stream.Read(buffer1, 0, (int)buffer1.Length);
                        process = (float)GetLength / (float)lengths * 100;
                    }
                    buffer = memory.ToArray();
                    memory.Close();
                }
                return buffer;
            }
            catch (Exception ex)
            {
              
                return null;
            }
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url">上传网址</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="saveName">上传文件的名字</param>
        /// <returns></returns>
        public static string UpLoadFiles(string url, string filePath, string saveName)
        {
            int returnValue = 0;
            //要上传的文件
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);
            //时间戳
            string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");
            //请求头部信息
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(strBoundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append("file");
            sb.Append("\"; filename=\"");
            sb.Append(saveName);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append("application/octet-stream");
            sb.Append("\r\n");
            sb.Append("\r\n");
            string strPostHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);

            // 根据uri创建HttpWebRequest对象 
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(url);
            httpReq.Method = "POST";

            //对发送的数据不使用缓存 
            httpReq.AllowWriteStreamBuffering = false;

            //设置获得响应的超时时间（300秒） 
            httpReq.Timeout = 300000;
            httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;
            long length = fs.Length + postHeaderBytes.Length + boundaryBytes.Length;
            long fileLength = fs.Length;
            httpReq.ContentLength = length;
            try
            {
                //每次上传4k 
                int bufferLength = 4096;
                byte[] buffer = new byte[bufferLength];

                //已上传的字节数 
                long offset = 0;

                //开始上传时间 
                DateTime startTime = DateTime.Now;
                int size = r.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();

                //发送请求头部消息 
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    offset += size;
                    TimeSpan span = DateTime.Now - startTime;
                    double second = span.TotalSeconds;
                   
                    if (second > 0.001)
                    {
                        
                    }
                    else
                    {
                       
                    }
                  
                    size = r.Read(buffer, 0, bufferLength);
                }
                return "上传结束";
            }
            catch (Exception ex)
            {
                return "网络错误(Network error)" + ex.Message;
            }
        }
    }
}
