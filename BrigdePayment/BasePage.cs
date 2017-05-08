using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace BrigdePayment
{
    public class BasePage : System.Web.UI.Page
    {
        string token = "abcd1234";
        protected string APP_ID = "wxb92981b6615910c7";
        protected string MCH_ID = "1400721802";
        protected string APP_KEY = "03941b924d12454219648d61a7b025e1";

        internal bool CheckSign(string requestParams)
        {
            string[] strRequest = requestParams.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            sb.Append(token);
            foreach (var item in strRequest)
            {
                sb.AppendFormat("&{0}={1}", item, HttpContext.Current.Request[item]);
            }
            string encryption = MD5Encrypt(sb.ToString().ToLower());
            string sign = HttpContext.Current.Request["sign"];
            if (encryption.ToLower() != sign)
            {
                ResultObj ret= new ResultObj("-1", "sign is invalid", null);
                outputJSON(ret);
                return false;
            }
            return true;
        }


        public static string MD5Encrypt(string strText)
        {
            byte[] result = Encoding.Default.GetBytes(strText);    //tbPass为输入密码的文本框  
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");
        }

        public void outputJSON(ResultObj obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            string output= jss.Serialize(obj);
            Response.Write(output);
        }


        public string nonce_str
        {
            get
            {
                return MD5Encrypt(DateTime.Now.Millisecond.ToString() + ":" + (new Random(888888)).ToString() + "-" + (new Random(888888)).ToString());
            }
        }

        public string calculateSign(Dictionary<string, string> param, string key)
        {
            Dictionary<string, string> newparam = param.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
            StringBuilder sb = new StringBuilder();
            foreach (var item in newparam)
            {
                if (item.Key != "sign" && item.Key != "key" && !string.IsNullOrEmpty(item.Value))
                {
                    sb.AppendFormat("{0}={1}&", item.Key, item.Value);
                }
            }
            string ret = sb.ToString().TrimEnd(new char[] { '&' }) + "&key=" + key;
            ret = MD5Encrypt(ret).ToUpper();
            return ret;
        }

        public string getXMLFromArray(Dictionary<string, string> arr)
        {
            StringBuilder sb = new StringBuilder("<xml>");
            foreach (var item in arr)
            {
                sb.AppendFormat("<{0}>{1}</{0}>", item.Key, item.Value);
            }
            sb.Append("</xml>");
            return sb.ToString();
        }
        public string postXmlCurl(string postdata, string url)
        {

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {

                request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.AllowAutoRedirect = false;   // 不自动跳转


                request.KeepAlive = true;

                byte[] postdatabytes = Encoding.UTF8.GetBytes(postdata);
                request.ContentLength = postdatabytes.Length;
                Stream stream;
                stream = request.GetRequestStream();

                stream.Write(postdatabytes, 0, postdatabytes.Length);
                stream.Close();

                response = (HttpWebResponse)request.GetResponse();


                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string content = sr.ReadToEnd();
                sr.Close();
                return content;
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                request.Abort();
                response.Close();
            }
            return "";
        }



        public class ResultObj
        {
            public string code = "";
            public string result = "";
            public object ret = null;
            public ResultObj(string code,string result, object ret)
            {
                this.code = code;
                this.result = result;
                this.ret = ret;
            }
        }
    }
}