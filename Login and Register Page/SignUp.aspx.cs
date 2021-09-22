using System;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Data;
using System.Configuration;
using System.Net.Mail;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;


namespace Login_and_Register_Page
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        string storedProcedure;
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

        private bool DomainExists(string input)
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(input.Substring(input.IndexOf("@") + 1));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected void buttonClicked(object sender, EventArgs args)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            //string ConnectionString = @"USER ID=chris; PASSWORD = chris; DATA SOURCE= XE; PERSIST SECURITY INFO=True";
            bool emailExists = false;
            OracleConnection con = new OracleConnection(ConnectionString);
            con.Open();
            OracleDataReader dataReader;
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = con;
            cmd.CommandText= "select email from info";
            cmd.CommandType = CommandType.Text;
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
            {
                if (emailExists == false)
                {
                    alreadyExists.Visible = false;
                    alreadyExists.Attributes.CssStyle.Add("display", "none");
                    if (password.Text != "" && passwordConfirm.Text != "")
                    {
                        missingFields.Visible = false;
                        missingFields.Attributes.CssStyle.Add("display", "none");
                        if (password.Text == passwordConfirm.Text)
                        {
                            passwordDontMatch.Visible = false;
                            passwordDontMatch.Attributes.CssStyle.Add("display", "none");
                            if (firstName.Text != "" && lastName.Text != "" && email.Text != "" && FileUpload1.HasFile)
                            {
                                missingFields.Visible = false;
                                missingFields.Attributes.CssStyle.Add("display", "none");
                                firstName.Text.Trim();
                                lastName.Text.Trim();
                                email.Text.Trim();
                                storedProcedure = "INSERT_PKG.insertNewAcc";
                                cmd.CommandText = storedProcedure;
                                cmd.CommandType = CommandType.StoredProcedure;
                                //cmd = new SqlCommand("insert into info(first,last,email,password,confirmpass,balance)values('" + firstName.Text + "','" + lastName.Text + "','" + email.Text + "','" + password.Text + "','" + passwordConfirm.Text + "','" + 0 + "')", con);
                                cmd.Parameters.Add("first", OracleDbType.Varchar2).Value = firstName.Text;
                                cmd.Parameters.Add("last", OracleDbType.Varchar2).Value = lastName.Text;
                                cmd.Parameters.Add("email", OracleDbType.Varchar2).Value = email.Text;
                                cmd.Parameters.Add("password", OracleDbType.Varchar2).Value = password.Text;
                                cmd.Parameters.Add("confirmpass", OracleDbType.Varchar2).Value = passwordConfirm.Text;
                                cmd.Parameters.Add("balance", OracleDbType.Decimal).Value = 0;
                                string fileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                                string filePath = "/Uploads/" + fileName;
                                FileUpload1.PostedFile.SaveAs(Server.MapPath(filePath));
                                cmd.Parameters.Add("filePath", OracleDbType.Varchar2).Value = filePath;
                                cmd.ExecuteNonQuery();
                                Server.Transfer("SignIn.aspx");
                                firstName.Text = "";
                                lastName.Text = "";
                                email.Text = "";
                                password.Text = "";
                                passwordConfirm.Text = "";
                            }
                            else
                            {
                                missingFields.Visible = true;
                                missingFields.Attributes.CssStyle.Add("display", "normal");
                            }
                        }
                        else
                        {
                            passwordDontMatch.Visible = true;
                            passwordDontMatch.Attributes.CssStyle.Add("display", "normal");
                        }
                    }
                    else
                    {
                        missingFields.Visible = true;
                        missingFields.Attributes.CssStyle.Add("display", "normal");
                    }
                }
                else
                {
                    alreadyExists.Visible = true;
                    alreadyExists.Attributes.CssStyle.Add("display", "normal");
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