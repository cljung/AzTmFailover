Param(
   [Parameter(Mandatory=$False)][string]$TrafficManagerName = "cljungtmpe01",            # TM name
   [Parameter(Mandatory=$False)][string]$ResourceGroupName = "cljungrgne01",             # Azure Resource Goup
   [Parameter(Mandatory=$False)][int]$Sleep = 10                                         # Sleep time in seconds between monitor calls
)

Do 
{
    $html = Invoke-WebRequest "http://$TrafficManagerName.trafficmanager.net/whoareyou.aspx" -Headers @{"Cache-Control"="no-cache"}
    write-output "$(get-date -format 'o')"
    write-output "Web site responding - $($html.Headers["WebSiteName"])"

    $ep = Get-AzureRmTrafficManagerProfile -Name $TrafficManagerName -resourceGroupName $ResourceGroupName
    foreach( $endpoint in $ep.Endpoints) {
      if ( $endpoint.EndpointMonitorStatus -ne "Online") {
        write-host "$($endpoint.Priority). $($endpoint.Target) - $($endpoint.EndpointMonitorStatus) ($($endpoint.Location))" -ForegroundColor Red
      } else {
        write-host "$($endpoint.Priority). $($endpoint.Target) - $($endpoint.EndpointMonitorStatus) ($($endpoint.Location))" 
      }
    }
    write-output ""
    Sleep $Sleep
} while(1 -eq 1)
