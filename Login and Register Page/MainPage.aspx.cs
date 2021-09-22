using System;
using System.Web;
using System.Web.UI;
using System.Net.Mail;
using System.Configuration;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace Login_and_Register_Page
{
    public partial class WebForm4 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Label1.Text = DateTime.Now.ToLongTimeString();
        }

        protected void SignIn(object sender, EventArgs e)
        {
            Server.Transfer("SignIn.aspx");
        }

        protected void SignUp(object sender, EventArgs e)
        {
            Server.Transfer("SignUp.aspx");
        }
    }
}