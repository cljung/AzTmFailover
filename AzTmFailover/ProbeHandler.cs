using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace AzTmFailover
{
    public class ProbeHandler
    {
        public static void SetStatusCode(int statusCode )
        {
            SetStatusCode(statusCode, false);
        }
        /// <summary>
        /// Sets the HTTP StatusCode that the probe handler should return
        /// </summary>
        /// <param name="statusCode"></param>
        public static void SetStatusCode( int statusCode, bool manuallySet )
        {
            HttpContext ctx = HttpContext.Current;
            if (ctx.Application["ProbeData"] != null)
            {
                ProbeData pd = ctx.Application["ProbeData"] as ProbeData;
                pd.statusCode = statusCode;
            }
        }
        /// <summary>
        /// Resets all saved data
        /// </summary>
        public static void ResetData()
        {
            HttpContext.Current.Application["ProbeData"] = null;
        }
        /// <summary>
        /// processes a probe request
        /// </summary>
        /// <returns></returns>
        public static int ProcessProbe()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            HttpContext ctx = HttpContext.Current;
            ProbeData pd;
            if (ctx.Application["ProbeData"] != null)
            {
                pd = ctx.Application["ProbeData"] as ProbeData;
            }
            else
            {
                pd = new ProbeData();
                pd.statusCode = 200;
                string siteName = Environment.ExpandEnvironmentVariables("%WEBSITE_SITE_NAME%");
                if (siteName.StartsWith("%"))
                    siteName = Environment.MachineName;
                pd.webServer = siteName;
                pd.lastFullHealthCheckUtc = DateTime.UtcNow.AddDays(-1); // a date not that far back in the history books
            }

            ProbeEvent pe = new ProbeEvent();
            pe.callerIp = ctx.Request.ServerVariables["REMOTE_HOST"].ToString();
            pe.timeUtc = DateTime.UtcNow;
            pe.fullCheck = false;
            pe.status = "";
            
            // since we may get hammered with lots of probe requests, let's just do a full health check every 30 seconds
            if ( pd.statusCode == 200 &&  (pe.timeUtc - pd.lastFullHealthCheckUtc).TotalSeconds >= 30 )
            {
                pd.lastFullHealthCheckUtc = pe.timeUtc;
                pe.fullCheck = true;
                string msg = "";
                if (!FullCheck(pd.webServer, out msg))
                     pd.statusCode = 500;
                else pd.statusCode = 200;
                pe.status = msg;
            }

            pe.statusCode = pd.statusCode;
            PushEventOnStack(pd, pe, 50);

            sw.Stop();
            pe.responseTimeMs = sw.ElapsedMilliseconds;
            ctx.Application["ProbeData"] = pd;

            //PrintEvent(pd);

            return pe.statusCode; // OK
        }
        /// <summary>
        /// Pushes a probe event data on to a stack with a finite size
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="pe"></param>
        /// <param name="sizeMax"></param>
        private static void PushEventOnStack( ProbeData pd, ProbeEvent pe, int sizeMax )
        {
            // only keep the last N probes in array
            // [8] becomes [9], [7] becomes [8], etc

            if (pd.probes == null)
            {
                pd.probes = new ProbeEvent[1];
            }
            else if ( pd.probes.Length < sizeMax )
            {
                ProbeEvent[] probes = new ProbeEvent[pd.probes.Length + 1];
                for( int n = pd.probes.Length; n > 0; n--)
                {
                    probes[n] = pd.probes[n - 1];
                }
                pd.probes = probes;
            }
            else
            {
                for (int n = pd.probes.Length; n > 1; n--)
                {
                    pd.probes[n-1] = pd.probes[n - 2];
                }
            }
            pd.probes[0] = pe;
        }

        /// <summary>
        /// Debug dump out the probe event data
        /// </summary>
        /// <param name="pd"></param>
        private static void PrintEvent(ProbeData pd )
        {
            int count = 0;
            foreach (var pe in pd.probes)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Probe[{0}]: {1}", count++, pe.timeUtc.ToString()));
            }
        }
        private static bool FullCheck( string website, out string msg )
        {
            bool rc = false;
            msg = "";
            System.Data.SqlClient.SqlConnection dbConn = null;
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                dbConn = new System.Data.SqlClient.SqlConnection(connectionString);
                dbConn.Open();

                bool readwrite = true;
                string sql = "";
                // if connection string indicates read-only access, then just do a read test on the db
                // if we are in read/write mode, make sure we can insert data to the db
                if ( connectionString.ToLowerInvariant().Contains("applicationintent=readonly"))
                {
                    readwrite = false;
                    sql = string.Format("select webserver, probetimeutc from TM_ReadWrite_test where 1=0");
                }
                else
                {
                    sql = string.Format("insert into TM_ReadWrite_test ( webserver, probetimeutc ) select '{0}', GETUTCDATE()\n" +
                                               "delete from TM_readWrite_test where datediff(minute, probetimeutc, getutcdate()) > 60"
                                               , website
                                               );
                }
                // try the query with a really aggressive timeout so that we don't clogg up the system
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, dbConn);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 5; 
                if ( readwrite )
                {
                    cmd.ExecuteNonQuery();
                    msg = string.Format("R/W OK: {0}.{1}", dbConn.DataSource, dbConn.Database);
                }
                else
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Close();
                    msg = string.Format("R/O OK: {0}.{1}", dbConn.DataSource, dbConn.Database);
                }
                rc = true;
            }
            catch( Exception ex )
            {
                msg = ex.Source + ": " + ex.Message;
            }
            finally
            {
                if ( dbConn != null)
                {
                    if (dbConn.State != ConnectionState.Closed)
                        dbConn.Close();
                }
            }
            return rc;
        }
    }
}