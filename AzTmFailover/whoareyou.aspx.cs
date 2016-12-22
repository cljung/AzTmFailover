using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AzTmFailover
{
    public partial class whoareyou : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                string siteName = Environment.ExpandEnvironmentVariables("%WEBSITE_SITE_NAME%");
                if (siteName.StartsWith("%"))
                    siteName = Environment.MachineName;
                Response.Write(siteName + " - " + DateTime.UtcNow.ToString());
                Response.Headers.Add("WebSiteName", siteName);
                Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
                Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
                Response.AppendHeader("Expires", "0"); // Proxies.
                Response.End();
            }
        }
    }
}