using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace BrigdePayment
{
    public partial class payment : BasePage
    {
        //define("APP_ID",  "wxb92981b6615910c7");
        // 商户号 (开户邮件中可查看)
        //define("MCH_ID",  '1400721802');
        //// 商户支付密钥 (https://pay.weixin.qq.com/index.php/account/api_cert)
        //define("APP_KEY",'03941b924d12454219648d61a7b025e1' );

        string APP_ID = "wxb92981b6615910c7";
        string MCH_ID = "1400721802";
        string APP_KEY = "03941b924d12454219648d61a7b025e1";

        string nonce_str
        {
            get
            {
                return MD5Encrypt(DateTime.Now.Millisecond.ToString()+":"+ (new Random(888888)).ToString()+"-"+ (new Random(888888)).ToString());
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            try
            {
                string orderno = HttpContext.Current.Request["orderno"];
                string customerid = HttpContext.Current.Request["customerid"];
                string amount = HttpContext.Current.Request["amount"];
                string subject = HttpContext.Current.Request["subject"];

                if (string.IsNullOrEmpty(orderno))
                {
                    outputJSON(new ResultObj("-101", "order no is required", null));
                }
                if (string.IsNullOrEmpty(customerid))
                {
                    outputJSON(new ResultObj("-102", "customer id is required", null));
                }
                if (string.IsNullOrEmpty(amount))
                {
                    outputJSON(new ResultObj("-103", "amount is required", null));
                }

                base.CheckSign("orderno,customerid,amount");

                string prepay_id = generatePrepayId(subject, amount, orderno);

                if (string.IsNullOrEmpty(prepay_id))
                {
                    outputJSON(new ResultObj("-301", "prepay_id generate fail", null));
                }
               
                response.Add("appid", APP_ID);
                response.Add("partnerid", MCH_ID);
                response.Add("prepayid", prepay_id);
                response.Add("package", "Sign=WXPay");
                response.Add("noncestr", nonce_str);
                response.Add("timestamp", DateTime.Now.Ticks.ToString());
                string sign = calculateSign(response, APP_KEY);
                response.Add("sign", sign);
            }
            catch (Exception ex)
            {
                outputJSON(new ResultObj("500", "generate fail", null));
            }

            outputJSON(new ResultObj("0", "success", response));
        }

        private string generatePrepayId(string subject, string amount, string orderno)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appid", APP_ID);
            param.Add("mch_id", MCH_ID);
            param.Add("nonce_str", nonce_str);
            param.Add("body", subject);
            param.Add("out_trade_no", orderno);
            param.Add("total_fee", (Convert.ToDecimal(amount) * 100).ToString());
            param.Add("spbill_create_ip", "1.2.3.4");
            param.Add("notify_url", HttpContext.Current.Request.Url.Host + "/notify.aspx");
            param.Add("trade_type", "APP");
            string sign = calculateSign(param,APP_KEY);
            param.Add("sign", sign);
            string xml = getXMLFromArray(param);
            string result= postXmlCurl(xml, "https://api.mch.weixin.qq.com/pay/unifiedorder");

            try
            {
                XDocument xdoc = XDocument.Parse(result);
                XElement xroot = xdoc.Root;
                if (xroot.Element("return_code").Value == "SUCCESS")
                {
                    return xroot.Element("prepay_id").Value;
                }
            }
            catch
            {

            }

            return "";

        }
        private string calculateSign(Dictionary<string, string> param,string key)
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
            string ret=sb.ToString().TrimEnd(new char[] { '&' })+"&key="+key;
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
    }
}