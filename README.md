# Azure Traffic Monitor Probe and failover demo

This is a sample WebApp that demonstrates the use of Traffic Manager and custom probes. You need to deploy it as a Azure AppServices WebApp into two different regions, then create a Traffic Manager resource that holds an endpoint to each WebApp.
Select one Azure Region as your promary and the other as your secondary. Give the TM endpoint of the primary Priority=1 and the other Priority=2.

You must configure the connect string in the azure Portal for each WebApp.

Create a SQL Azure database in your primary region and geo-replicate that to your secondary region as readable.

The folder named SQL contains a small DDL script for creating a small table that the probe uses to check database health.

The ps folder contains a small PowerShell script to monitor the TM endpoints status.

You can simulate a WebApp failure by passing default.aspx/?statusCode=404 as a query string

Please see <a href="http://www.redbaronofazure.com/?p=7316">this blob post</a> for an explanation of what is going on. 
