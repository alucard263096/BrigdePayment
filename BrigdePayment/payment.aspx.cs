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
                response.Add("noncestr", nonce_str);
                response.Add("timestamp", DateTime.Now.Ticks.ToString());
                string sign = calculateSign(response, APP_KEY);
                response.Add("sign", sign);
            }
            catch (Exception ex)
            {
                outputJSON(new ResultObj("500", "generate fail", null));
                return;
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
            string notifyurl = "http://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port 
                + "/notify.aspx";
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
        
    }
}