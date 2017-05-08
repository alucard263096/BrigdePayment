using AppLink.api;
using AppLink.core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace BrigdePayment
{
    public partial class query : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string orderno = HttpContext.Current.Request["orderno"];
                if (string.IsNullOrEmpty(orderno))
                {
                    outputJSON(new ResultObj("-101", "order no is required", null));
                    return;
                }

                if (base.CheckSign("orderno") == false)
                {
                    return;
                }

                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("appid", APP_ID);
                param.Add("mch_id", MCH_ID);
                param.Add("nonce_str", nonce_str);
                param.Add("out_trade_no", orderno);
                string sign = calculateSign(param, APP_KEY);
                param.Add("sign", sign);
                string xml = getXMLFromArray(param);
                string result = postXmlCurl(xml, "https://api.mch.weixin.qq.com/pay/orderquery");

                try
                {
                    XDocument xdoc = XDocument.Parse(result);
                    XElement xroot = xdoc.Root;
                    if (xroot.Element("return_code").Value == "SUCCESS")
                    {
                        if (xroot.Element("result_code").Value == "SUCCESS")
                        {
                            List<Param> lst = new List<Param>();
                            lst.Add(new Param("orderno", orderno));
                            DataTable dt=PaymentMgr.list(DBFactory.GetInstance(), lst);
                            int primary_id = 0;
                            if (dt.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (dr["orderno"].ToString() == orderno)
                                    {
                                        primary_id = Convert.ToInt32(dr["id"]);
                                    }
                                }
                            }
                            lst.Add(new Param("time_end", xroot.Element("time_end").Value));
                            lst.Add(new Param("openid", xroot.Element("openid").Value));
                            lst.Add(new Param("total_fee", xroot.Element("total_fee").Value));
                            lst.Add(new Param("trade_type", xroot.Element("trade_type").Value));
                            lst.Add(new Param("transaction_id", xroot.Element("transaction_id").Value));
                            lst.Add(new Param("result_code", xroot.Element("result_code").Value));

                            PaymentMgr.update(DBFactory.GetInstance(), lst);

                            outputJSON(new ResultObj("0", "SUCCESS", xroot.Element("trade_state").Value));
                        }
                        else
                        {
                            outputJSON(new ResultObj("-1", "FAIL", xroot.Element("result_code").Value));
                        }
                    }
                    else
                    {
                        outputJSON(new ResultObj("-301", "connect to wechat pay fail", result));
                    }
                    return;
                }
                catch
                {
                    outputJSON(new ResultObj("-301", "connect to wechat pay fail", result));
                }
            }
            catch (Exception ex)
            {
                outputJSON(new ResultObj("500", "query fail", null));
                return;
            }
        }
    }
}