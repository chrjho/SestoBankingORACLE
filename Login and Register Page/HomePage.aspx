<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="Login_and_Register_Page.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sesto Banking</title>
    <link href="styleCSShome.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="colorTop" runat="server">
            <asp:Image id="sesto" src="SestoCropped.png" runat="server" />
            <asp:Button ID="logOutButton" Text="Log Out" runat="server" OnClick="logOut" />
        </div>
        <br />
        <div id="PageBody" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:Timer ID="Timer1" runat="server" OnTick="Timer1_Tick" Interval="1000"></asp:Timer>
                    <asp:Label ID="Label1" runat="server"></asp:Label>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:Label ID="balanceLabel" runat="server" Text="Your Balance:"></asp:Label>
            <asp:Label ID="balance" runat="server" Text=""></asp:Label>
            <br />
            <asp:Button ID="dButton" runat="server" Text="Deposit Money" Onclick="depositButton"/>
            <asp:TextBox ID="dAmount" runat="server" placeholder="Amount" Visible="false"></asp:TextBox>
            <asp:Button ID="deposit" runat="server" Text="Deposit" Visible="false" Onclick="depositAmount"/>
            <asp:Button ID="cancelD" runat="server" Text="Cancel" Visible="false" OnClick="cancelDeposit" />
            <br />
            <asp:Button ID="wButton" runat="server" Text="Withdraw Money" Onclick="withdrawButton"/>
            <asp:TextBox ID="wAmount" runat="server" placeholder="Amount" Visible="false"></asp:TextBox>
            <asp:Button ID="withdraw" runat="server" Text="Withdraw" Visible="false" Onclick="withdrawAmount"/>
            <asp:Button ID="cancelW" runat="server" Text="Cancel" Visible="false" OnClick="cancelWithdraw" />
            <label id="insufficientFundsW" runat="server" visible="false">Insufficient funds.</label>
            <br />
            <asp:Button ID="send" runat="server" Text="Send Money" Onclick="sendButton"/>
            <asp:TextBox ID="sendTo" runat="server" placeholder="Recepient's E-mail" Visible="false"></asp:TextBox>
            <asp:TextBox ID="sendAmount" runat="server" placeholder="Amount" Visible="false"></asp:TextBox>
            <asp:Button ID="sendMoney" runat="server" Text="Send" Visible="false" Onclick="sendMoneyToUser"/>
            <asp:Button ID="cancelS" runat="server" Text="Cancel" Visible="false" OnClick="cancelSend" />
            <label id="insufficientFunds" runat="server" visible="false">Insufficient funds.</label>
            <label id="invalidUser" runat="server" visible="false">Enter valid E-mail.</label>
            <br />
            <select id="selectMonth" placeholder="-Select-" runat="server">
                <option value=''>--Select Month--</option>
                <option value='1'>Janaury</option>
                <option value='2'>February</option>
                <option value='3'>March</option>
                <option value='4'>April</option>
                <option value='5'>May</option>
                <option value='6'>June</option>
                <option value='7'>July</option>
                <option value='8'>August</option>
                <option value='9'>September</option>
                <option value='10'>October</option>
                <option value='11'>November</option>
                <option value='12'>December</option>
            </select>
            <select id="selectYear" placeholder="-Select-" runat="server">
                <option value=''>--Select Year--</option>
                <option value="2000">2000</option>
                <option value="2001">2001</option>
                <option value="2002">2002</option>
                <option value="2003">2003</option>
                <option value="2004">2004</option>
                <option value="2005">2005</option>
                <option value="2006">2006</option>
                <option value="2007">2007</option>
                <option value="2008">2008</option>
                <option value="2009">2009</option>
                <option value="2010">2010</option>
                <option value="2011">2011</option>
                <option value="2012">2012</option>
                <option value="2013">2013</option>
                <option value="2014">2014</option>
                <option value="2015">2015</option>
                <option value="2016">2016</option>
                <option value="2017">2017</option>
                <option value="2018">2018</option>
                <option value="2019">2019</option>
                <option value="2020">2020</option>
                <option value="2021">2021</option>
            </select>
            <asp:Button ID="viewAll" runat="server" Text="View Transactions" OnClick="viewAllTransactions" />
            <br />
            <label id="selectError" runat="server" visible="false">Please select a month and year.</label>
            <br />
            <label id="noResults" runat="server" visible="false">No results were found.</label>
            <asp:PlaceHolder ID = "PlaceHolder" runat="server" />
            <asp:Button ID="prev" runat="server" Text="Previous Page" OnClick="PreviousRecords"/>
            <label id="page" runat="server">Page </label>
            <label id="pageNumber" runat="server"></label>
            <asp:Button ID="next" runat="server" Text="Next Page" OnClick="NextRecords"/>

            <table id="dtHTML" runat="server"></table>
       </div>
   </form>
</body>
</html>
