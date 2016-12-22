using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AzTmFailover
{
    public partial class probe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // this page must do a 200 OK return or else Traffic Manager will start the failover procedure
            if ( !this.IsPostBack )
            {
                int statusCode = ProbeHandler.ProcessProbe();
                Response.Write(statusCode.ToString() + ": probed " + DateTime.UtcNow.ToString() );
                Response.StatusCode = statusCode;
                Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
                Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
                Response.AppendHeader("Expires", "0"); // Proxies.
                Response.End();
            }
        }
    }
}