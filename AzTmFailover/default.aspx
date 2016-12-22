<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="AzTmFailover._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Azure Traffic Manager Probe Demo</title>
</head>
<body style="font-family: Arial, veranda; font-size: 14px; background-color: darkgrey">
    <form id="form1" runat="server">
    <div>
    <h1>Azure Traffic Manager Probing Demo</h1>
    <h2>Probes on <asp:Label runat="server" ID="ltWebSite"></asp:Label></h2>
    <table border="1">
        <tr style="font-weight: bold; font-size: 18px;">
            <td>Seq</td><td>TimeUTC</td><td>Web Server</td><td>Caller IP</td><td>ms</td><td>Full Check</td><td>StatusCode</td><td>Status</td>
            <asp:Literal runat="server" ID="ltRows" />
        </tr>
    </table>
        
    </div>
    </form>
</body>
</html>
