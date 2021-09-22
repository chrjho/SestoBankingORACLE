<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="Login_and_Register_Page.WebForm3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sesto Banking</title>
    <link href="styleCSSsignin.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="colorTop">
                <asp:Image id="sesto" src="SestoCropped.png" runat="server" />
        </div>
        <div id="PageBody">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:Timer ID="Timer1" runat="server" OnTick="Timer1_Tick" Interval="1000"></asp:Timer>
                    <asp:Label ID="Label1" runat="server"></asp:Label>
                </ContentTemplate>
            </asp:UpdatePanel>
            <label id="invalidUser" runat="server" visible="false">No account associated with this E-mail.</label>
            <label id="incorrectPassword" runat="server" visible="false">Incorrect password.</label>
            <br />
            <asp:Label ID="emailLabel" runat="server" Text="E-mail"></asp:Label>
            <br />
            <asp:TextBox ID="email" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="passwordLabel" runat="server" Text="Password"></asp:Label>
            <br />
            <asp:TextBox ID="password" runat="server" TextMode="Password"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="login" runat="server" Text="Login" OnClick="Login" />
            <br />
            <asp:HyperLink id="signUp" Text="Don't have an account? Sign Up." NavigateUrl="SignUp.aspx" runat="server"/>
        </div>
    </form>
</body>
</html>
