using System;
using System.Web;
using System.Web.UI;
using System.Net.Mail;
using System.Configuration;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;


namespace Login_and_Register_Page
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = DateTime.Now.ToLongTimeString();
        }


        private bool IsValidEmailId(string inputEmail)
        {
            try
            {
                var m = new MailAddress(inputEmail);
                return true; 
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected void Login(object sender, EventArgs args)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            bool emailExists = false;
            OracleConnection con = new OracleConnection(ConnectionString);
            con.Open();
            OracleDataReader dataReader;
            OracleCommand cmd = new OracleCommand("select email from info", con);
            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                System.Diagnostics.Debug.WriteLine(dataReader.GetString(0));
                if (dataReader.GetString(0) == email.Text)
                {
                    emailExists = true;
                }
            }
            dataReader.Close();
            if (email.Text != null)
            {
                if (emailExists)
                {
                    invalidUser.Visible = false;
                    invalidUser.Attributes.CssStyle.Add("display", "none");
                    string curEmail = email.Text;
                    cmd = new OracleCommand("SELECT password FROM info WHERE email='" + curEmail + "'", con);
                    object pass = cmd.ExecuteScalar();
                    //var md5 = EasyEncryption.MD5.ComputeMD5Hash(password.Text);
                    //System.Diagnostics.Debug.WriteLine(md5);
                    if (pass != null)
                    {
                        string hashed = Convert.ToString(pass);
                        Console.WriteLine(hashed);
                        System.Diagnostics.Debug.WriteLine("this is hashed " + hashed);
                        if (hashed == password.Text)
                        {
                            incorrectPassword.Visible = false;
                            incorrectPassword.Attributes.CssStyle.Add("display", "none");
                            Session["email"] = email.Text;
                            Server.Transfer("HomePage.aspx");
                        }
                        else
                        {
                            incorrectPassword.Visible = true;
                            incorrectPassword.Attributes.CssStyle.Add("display", "normal");
                        }
                    }
                }
                else
                {
                    invalidUser.Visible = true;
                    invalidUser.Attributes.CssStyle.Add("display", "normal");
                    incorrectPassword.Visible = false;
                    incorrectPassword.Attributes.CssStyle.Add("display", "none");
                }
            }
            con.Close();
        }
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Label1.Text = DateTime.Now.ToLongTimeString();
        }
    }
}