using AppLink.api;
using AppLink.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BrigdePayment
{
    public partial class notify : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<Param> lstParam = new List<Param>();
            lstParam.Add(new Param("notify_time",DateTime.Now));

            JavaScriptSerializer jss = new JavaScriptSerializer();
            string log_request = jss.Serialize(HttpContext.Current.Request.Form);

            lstParam.Add(new Param("log_url", HttpContext.Current.Request.Url.AbsoluteUri));
            lstParam.Add(new Param("log_request", log_request));
            NotifylogMgr.update(DBFactory.GetInstance(), lstParam);




        }
    }
}