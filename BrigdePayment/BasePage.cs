using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace BrigdePayment
{
    public class BasePage : System.Web.UI.Page
    {
        string token = "abcd1234";

        internal void CheckSign(string requestParams)
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
            }
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
            Response.End();
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