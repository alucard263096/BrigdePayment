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
                    return;
                }
                if (string.IsNullOrEmpty(customerid))
                {
                    outputJSON(new ResultObj("-102", "customer id is required", null));
                    return;
                }
                if (string.IsNullOrEmpty(amount))
                {
                    outputJSON(new ResultObj("-103", "amount is required", null));
                    return;
                }

                if (base.CheckSign("orderno,customerid,amount") == false)
                {
                    return;
                }

                string prepay_id = generatePrepayId(subject, amount, orderno);
                
                response.Add("appid", APP_ID);
                response.Add("partnerid", MCH_ID);
                response.Add("prepayid", prepay_id);
                response.Add("package", "Sign=WXPay");
                response.Add("noncestr", createNonceStr());
                response.Add("timestamp", Convert.ToString(convertDateTimeInt(DateTime.Now)));
                string sign = calculateSign(response, APP_KEY);
                response.Add("sign", sign);
                outputJSON(new ResultObj("0", "success", response));
                return;
            }
            catch (Exception ex)
            {
                outputJSON(new ResultObj("500", "generate fail", null));
                return;
            }

        }

        private string generatePrepayId(string subject, string amount, string orderno)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("appid", APP_ID);
            param.Add("mch_id", MCH_ID);
            param.Add("nonce_str", createNonceStr());
            param.Add("body", subject);
            param.Add("out_trade_no", orderno);
            param.Add("total_fee", Convert.ToInt32((Convert.ToDecimal(amount) * 100)).ToString());
            param.Add("spbill_create_ip", "1.2.3.4");
            string notifyurl = "http://" + HttpContext.Current.Request.Url.Host  + "/notify.aspx";
            param.Add("notify_url", notifyurl);
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
                outputJSON(new ResultObj("-301", "prepay_id generate fail", result));
            }

            return "";

        }

        /// <summary>
        /// 创建随机字符串
        /// </summary>
        /// <returns></returns>
        private static string createNonceStr()
        {
            int length = 16;
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string str = "";
            Random rad = new Random();
            for (int i = 0; i < length; i++)
            {
                str += chars.Substring(rad.Next(0, chars.Length - 1), 1);
            }
            return str;
        }

        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>double</returns>  
        public static int convertDateTimeInt(System.DateTime time)
        {
            int intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = Convert.ToInt32((time - startTime).TotalSeconds);
            return intResult;
        }
    }
}