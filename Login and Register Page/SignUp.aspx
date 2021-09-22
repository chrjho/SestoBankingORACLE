<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="Login_and_Register_Page.WebForm2" %>

<!DOCTYPE html>

<html>
    <head runat="server">
	<title>Sesto Banking</title>
    <link href="styleCSS.css" rel="stylesheet" type="text/css" />
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
                        <br />
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:Label ID="alreadyExists" Text="Already an account associated with this email." runat="server" Visible="false"/>
                <label id="passwordDontMatch" runat="server" visible="false">Passwords do not match.</label>
                <label id="missingFields" runat="server" visible="false">Please fill in all fields and upload a profile picture.</label>
                <br>
                <asp:Label id="firstNameLabel" Text="First Name" runat="server"/>
                <asp:Label id="lastNameLabel" Text="Last Name" runat="server"/>
                <br>
                <asp:TextBox id="firstName" runat="server"/>
                <asp:TextBox id="lastName" runat="server"/>
                <br>
                <asp:Label id="emailLabel" Text="E-mail" runat="server"/>
                <br>
                <asp:TextBox id="email" runat="server"/>
                <br>
                <asp:Label id="passwordLabel" Text="Password" runat="server"/>
                <br>
                <asp:TextBox id="password" TextMode="Password" runat="server"/>
                <br>
                <asp:Label id="passwordConfirmLabel" Text="Confirm Password" runat="server"/>
                <br>
                <asp:TextBox id="passwordConfirm" TextMode="Password" runat="server"/>
                <br>
                <br />
                <label id="uploadPicLabel" runat="server">Please upload a profile picture.</label>
                <br />
                <asp:FileUpload ID="FileUpload1" runat="server" />
                <br />
                <br />
                <asp:Button id="button" runat="server" Text="Sign Up" OnClick="buttonClicked"/>
                <br>
                <asp:HyperLink id="haveAccount" Text="Already have an account?" NavigateUrl="SignIn.aspx" runat="server"/>
                <br />
                <br />
            </div>
	    </form>
    </body>
</html>
