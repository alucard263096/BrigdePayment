using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace AppLink.core
{

    public class APIInstance
    {
        public delegate void CallbackDelegate(string url,List<Param> param,Object dt);
        
        public string URLHeader { get; set; }

        public APIInstance(string urlhead)
        {
            URLHeader = urlhead;
        }


        private CookieContainer _cookieContainer;
        public CookieContainer Cookie
        {
            get;
            set;
        }
        private string _cookieStr;

        public void CallApiAsync(string url, List<Param> post,CallbackDelegate callbackmethod)
        {
            HttpWebRequest request = null;
            try
            {
                url = URLHeader + url;
                string postdata = ChangePostToStr(post);

                string sign = "";
                string md5str = "";
                string fmd5str = "";
                if (!string.IsNullOrEmpty(TOKEN) && !string.IsNullOrEmpty(RID))
                {
                    md5str = url + "~" + postdata + "~" + TOKEN + "~" + RID;
                    fmd5str = md5str;
                    md5str = md5str.ToUpper();
                    sign = Utils.MD5Encrypt(md5str + SALT);
                }

                request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Sign", "sign");
                request.Headers.Add("TokenKey", RID);
                request.Headers.Add("Fmd5str", fmd5str);
                request.Headers.Add("lang", LANG);
                request.AllowAutoRedirect = false;   // 不自动跳转

                if (_cookieContainer != null)
                {
                    request.CookieContainer = _cookieContainer;
                }
                else
                {
                    request.CookieContainer = new CookieContainer();
                    _cookieContainer = request.CookieContainer;
                }
                request.KeepAlive = true;

                byte[] postdatabytes = Encoding.UTF8.GetBytes(postdata);
                request.ContentLength = postdatabytes.Length;
                Stream stream;
                stream = request.GetRequestStream();

                stream.Write(postdatabytes, 0, postdatabytes.Length);
                stream.Close();

                CallObject call = new CallObject(callbackmethod,this,url,post);

                request.BeginGetResponse(new AsyncCallback(call.GetResponseCallback), request);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                request.Abort();
            }
        }
        
        public Object CallApi(string url,List<Param> post)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                url = URLHeader + url;
                string postdata = ChangePostToStr(post);

                string sign = "";
                string md5str = "";
                string fmd5str = "";
                if (!string.IsNullOrEmpty(TOKEN) && !string.IsNullOrEmpty(RID))
                {
                    md5str = url + "~" + postdata + "~" + TOKEN + "~" + RID;
                    fmd5str = md5str;
                    md5str = md5str.ToUpper();
                    sign = Utils.MD5Encrypt(md5str + SALT);
                }

                request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Sign", sign);
                request.Headers.Add("TokenKey", RID);
                request.Headers.Add("Fmd5str", fmd5str);
                request.Headers.Add("lang", LANG);
                request.AllowAutoRedirect = false;   // 不自动跳转

                if (_cookieContainer != null)
                {
                    request.CookieContainer = _cookieContainer;
                }
                else
                {
                    request.CookieContainer = new CookieContainer();
                    _cookieContainer = request.CookieContainer;
                }
                request.KeepAlive = true;

                byte[] postdatabytes = Encoding.UTF8.GetBytes(postdata);
                request.ContentLength = postdatabytes.Length;
                Stream stream;
                stream = request.GetRequestStream();

                stream.Write(postdatabytes, 0, postdatabytes.Length);
                stream.Close();

                response = (HttpWebResponse)request.GetResponse();

                response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);
                CookieCollection cook = response.Cookies;
                string strcrook = request.CookieContainer.GetCookieHeader(request.RequestUri);
                _cookieStr = strcrook;

                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string content = sr.ReadToEnd();
                sr.Close();
                return ToDataTable(content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                request.Abort();
                response.Close();
            }
        }

        public static string ChangePostToStr(List<Param> post)
        {
            if (post == null)
            {
                return "";
            }
            var orderList = post.OrderBy(r => r.Name);
            post = orderList.ToList();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < post.Count(); i++)
            {
                Param item = post[i];
                if (i > 0)
                {
                    sb.Append("&");
                }
                sb.Append(item.Name + "=" + Uri.EscapeUriString(Convert.ToString(item.Value)));
            }
            return sb.ToString();
        }

        #region Json 字符串 转换为 DataTable数据集合
        /// <summary>
        /// Json 字符串 转换为 DataTable数据集合
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Object ToDataTable(string json)
        {
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
                Object jsonobj = javaScriptSerializer.Deserialize<Object>(json);
                return jsonobj;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private string TOKEN { set; get; }
        private string RID { set; get; }

        public void SetToken(string token,string rid)
        {
            TOKEN = token;
            RID = rid;
        }

        private string LANG { set; get; }

        public void SetLang(string lang)
        {
            LANG = lang;
        }

        private string SALT = "";


        class CallObject{
            private CallbackDelegate callback;
            private APIInstance api;
            private string URL ;
            private List<Param> PARAM;
            public CallObject(CallbackDelegate callback,APIInstance api,string url,List<Param> param)
            {
                this.callback += callback;
                this.api = api;
                this.URL = url;
                this.PARAM = param;
            }

            public void GetResponseCallback(IAsyncResult asynchronousResult)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);
                    CookieCollection cook = response.Cookies;
                    string strcrook = request.CookieContainer.GetCookieHeader(request.RequestUri);
                    api._cookieStr = strcrook;

                    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    string content = sr.ReadToEnd();
                    sr.Close();
                    Object dt= ToDataTable(content);
                    this.callback(URL, PARAM, dt);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
    
}
