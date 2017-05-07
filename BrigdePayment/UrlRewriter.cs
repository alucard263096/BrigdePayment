using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
/// <summary>  
/// UrlRewriter URL重写类  
/// http://blog.csdn.net/jxqvip/article/details/6710904#
/// </summary>  
/// 

namespace BridgePayment
{

    public class UrlRewriter : IHttpHandler //实现“IHttpHandler”接口  
    {
        public UrlRewriter()
        {
            //  
            // TODO: 在此处添加构造函数逻辑  
            //  
        }
        public void ProcessRequest(HttpContext Context)
        {
            try
            {
                //取得原始URL屏蔽掉参数  
                string Url = Context.Request.RawUrl;
                //建立正则表达式  
                System.Text.RegularExpressions.Regex Reg =
                    new System.Text.RegularExpressions.Regex(@"/payment", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //(@"/show-(\d+)-(\d+)\..+", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //用正则表达式进行匹配  
                System.Text.RegularExpressions.Match m = Reg.Match(Url, Url.LastIndexOf("/"));//从最后一个“/”开始匹配  
                if (m.Success)//匹配成功  
                {
                    //String RealPath = @"~/payment.aspx";//?type=" + m.Groups[1] + "&id=" + m.Groups[2];
                    //Context.Response.Write(RealPath);  
                    //Context.RewritePath(RealPath);//(RewritePath 用在无 Cookie 会话状态中。)  
                    //Context.Server.Execute(RealPath);

                    Context.Response.Write("hello");
                    Context.Response.End();
                }
                else

                {
                    Context.Response.Redirect(Context.Request.Url.ToString());
                }
            }
            catch
            {
                Context.Response.Redirect(Context.Request.Url.ToString());
            }
        }
        /// <summary>  
        /// 实现“IHttpHandler”接口所必须的成员  
        /// </summary>  
        /// <value></value>  
        public bool IsReusable
        {
            get { return false; }
        }
    }

}