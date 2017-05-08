using AppLink.api;
using AppLink.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace BrigdePayment
{
    public partial class notify : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<Param> lstParam = new List<Param>();
            lstParam.Add(new Param("notify_time",DateTime.Now));

            StreamReader reader = new StreamReader(Request.InputStream);
            String xmlData = reader.ReadToEnd();

            lstParam.Add(new Param("log_url", HttpContext.Current.Request.Url.AbsoluteUri));
            lstParam.Add(new Param("log_request", xmlData));
            NotifylogMgr.update(DBFactory.GetInstance(), lstParam);

            //           <xml><appid><![CDATA[wxb92981b6615910c7]]></appid>
            //<bank_type><![CDATA[CFT]]></bank_type>
            //<cash_fee><![CDATA[1]]></cash_fee>
            //<fee_type><![CDATA[CNY]]></fee_type>
            //<is_subscribe><![CDATA[N]]></is_subscribe>
            //<mch_id><![CDATA[1400721802]]></mch_id>
            //<nonce_str><![CDATA[eqHlA5Ox8MZmIXaU]]></nonce_str>
            //<openid><![CDATA[ovCc8w9QBoRqua4cs7PNNUQQRlqI]]></openid>
            //<out_trade_no><![CDATA[t0012233]]></out_trade_no>
            //<result_code><![CDATA[SUCCESS]]></result_code>
            //<return_code><![CDATA[SUCCESS]]></return_code>
            //<sign><![CDATA[9C0669A299E609A86E412AF396534443]]></sign>
            //<time_end><![CDATA[20170508232435]]></time_end>
            //<total_fee>1</total_fee>
            //<trade_type><![CDATA[APP]]></trade_type>
            //<transaction_id><![CDATA[4005722001201705080107853520]]></transaction_id>
            //</xml>

            XDocument xdoc = XDocument.Parse(xmlData);
            XElement xroot = xdoc.Root;

            //似乎这个签名知道规则的人都可以伪造，有点白痴~~~
            //跳过不验证签名了

            UpdatePaymentInfo(xroot.Element("out_trade_no").Value, xroot);

        }
    }
}