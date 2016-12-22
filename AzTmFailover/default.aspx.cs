using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace AzTmFailover
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ( !this.IsPostBack )
            {
                string sc = Request.QueryString["statusCode"];
                int statusCode = 200;
                if (!string.IsNullOrEmpty(sc) && int.TryParse( sc, out statusCode) )
                {
                    ProbeHandler.SetStatusCode(statusCode);
                }

                string reset = Request.QueryString["reset"];
                if (!string.IsNullOrEmpty(reset))
                {
                    ProbeHandler.ResetData();
                }

                string siteName = Environment.ExpandEnvironmentVariables("%WEBSITE_SITE_NAME%");
                if (siteName.StartsWith("%"))
                    siteName = Environment.MachineName;
                Response.Headers.Add("WebSiteName", siteName);
                Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
                Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
                Response.AppendHeader("Expires", "0"); // Proxies.
                ltWebSite.Text = siteName;

                DrawProbeDetails();
            }
        }
        private void DrawProbeDetails()
        {
            HttpContext ctx = HttpContext.Current;
            ProbeData pd;
            if (ctx.Application["ProbeData"] == null)
                return;

            StringBuilder sb = new StringBuilder();
            int count = 0;
            pd = ctx.Application["ProbeData"] as ProbeData;
            string[] styles = new string[] { "", " style=\"color: darkred;\"" };
            foreach( var pe in pd.probes )
            {
                count++;
                int idx = (pe.statusCode == 200) ? 0 : 1;
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td{6}>{7}</td><td>{8}</td></tr>"
                            , count, pe.timeUtc.ToString(), pd.webServer, pe.callerIp, pe.responseTimeMs, pe.fullCheck, styles[idx], pe.statusCode, pe.status);
            }
            ltRows.Text = sb.ToString();
        }
    }
}