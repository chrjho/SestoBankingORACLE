using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data;
using System.Configuration;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace Login_and_Register_Page
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        string curUser;
        StringBuilder table;
        int accNum;
        int pageNum;
        int numPages;
        static string holdMonth;
        static string holdYear;
        string storedProcedure;
        protected void Page_Load(object sender, EventArgs e)
        {
            curUser = Session["email"].ToString();
            System.Diagnostics.Debug.WriteLine(curUser);
            string ConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            OracleConnection con = new OracleConnection(ConnectionString);
            con.Open();
            Label1.Text = DateTime.Now.ToLongTimeString();
            //OracleCommand cmd = new OracleCommand("SELECT balance FROM info WHERE email='" + curUser + "'", con);
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = con;
            storedProcedure = "FUNCTION_PKG.getBalanceFunc";
            cmd.CommandText = storedProcedure;
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            cmd.Parameters.Add("retBalance", OracleDbType.Decimal).Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
            cmd.ExecuteScalar();
            object bal = cmd.Parameters["retBalance"].Value;
            decimal balanceT = Convert.ToDecimal(bal.ToString());

            /*if (bal != null)
            {
                double cur = Convert.ToDouble(bal);
                Console.WriteLine(cur);
                balance.Text = "$" + cur;
            }*/
            balance.Text = "$" + balanceT;
            table = new StringBuilder();
            //cmd = new OracleCommand("select accNum from info where email='" + curUser + "'", con);
            storedProcedure = "SELECT_PKG.getAccNum";
            cmd.CommandText = storedProcedure;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("curAccNum", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
            object readAccNum = cmd.ExecuteScalar();
            accNum = Convert.ToInt32(readAccNum);
            storedProcedure = "SELECT_PKG.showUsingPaging";
            //cmd = new OracleCommand("with selectRes as (select transactions.transact, transactions.amount, transactions.time from transactions where transactions.accNum = '"+accNum+"' union all select transfers.transact, transfers.amount, transfers.time from transfers where transfers.accNum = '"+accNum+"')select top 10 * from selectRes order by time desc", con);
            cmd.CommandText = storedProcedure;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            OracleParameter output = cmd.Parameters.Add("retData", OracleDbType.RefCursor);
            output.Direction = ParameterDirection.Output;
            if (Convert.ToInt64(Session["PageNum"]) == 0)
            {
                cmd.Parameters.Add("PageNumber", OracleDbType.Int64).Value = 1;
            }
            else
            {
                cmd.Parameters.Add("PageNumber", OracleDbType.Int64).Value = Convert.ToInt64(Session["PageNum"]);
            }
            cmd.Parameters.Add("accNumber", OracleDbType.Int64).Value = accNum;
            holdMonth = Convert.ToString(Session["holdMonth"]);
            holdYear = Convert.ToString(Session["holdYear"]);
            cmd.Parameters.Add("month", OracleDbType.Varchar2).Value = holdMonth;
            cmd.Parameters.Add("year", OracleDbType.Varchar2).Value = holdYear;
            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
            DataTable displayTable = new DataTable();
            adapter.Fill(displayTable);
            DataToHTML(displayTable,accNum);
            storedProcedure = "SELECT_PKG.getPageCount";
            cmd.CommandText = storedProcedure;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("curPageCount", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("INaccNum", OracleDbType.Int64).Value=accNum;
            object NumPages = cmd.ExecuteScalar();
            numPages= (Convert.ToInt32(NumPages)-1) /20 + 1;
            if (Convert.ToString(Session["pageNum"]) == "")
            {
                pageNumber.InnerText = "1/" + numPages + "";
            }
            else
            {
                pageNumber.InnerText = "" + Convert.ToString(Session["pageNum"]) + "/" + numPages + "";
            }
            con.Close();
        }

        protected void depositAmount(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(dAmount.Text))
            {
                bool isNumeric = decimal.TryParse(dAmount.Text, out _);
                if (isNumeric)
                {
                    double value = Convert.ToDouble(dAmount.Text);
                    double cur;
                    double newBalance;
                    if (value > 0)
                    {
                        string ConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                        OracleConnection con = new OracleConnection(ConnectionString);
                        con.Open();
                        //OracleCommand cmd = new OracleCommand("SELECT balance FROM info WHERE email='" + curUser + "'", con);
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = con;
                        storedProcedure = "SELECT_PKG.getBalance";
                        cmd.CommandText = storedProcedure;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("curBalance", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
                        object bal = cmd.ExecuteScalar();
                        //double cur;
                        if (bal != null)
                        {
                            cur = Convert.ToDouble(bal);
                            Console.WriteLine(cur);
                            balance.Text = "$" + cur;
                            //cmd = new OracleCommand("UPDATE info SET balance='" + (cur + value) + "' WHERE email='" + curUser + "'", con);
                            storedProcedure = "UPDATE_PKG.updateBalanceAdd";
                            cmd.CommandText = storedProcedure;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("addX", OracleDbType.Decimal).Value = cur;
                            cmd.Parameters.Add("addY", OracleDbType.Decimal).Value = value;
                            cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
                            cmd.ExecuteNonQuery();
                            //cmd = new OracleCommand("select accNum from info where email='" + curUser + "'", con);
                            storedProcedure = "SELECT_PKG.getAccNum";
                            cmd.CommandText = storedProcedure;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("curAccNum", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
                            object readAccNum = cmd.ExecuteScalar();
                            accNum = Convert.ToInt32(readAccNum);
                            //cmd = new OracleCommand("insert into transactions(accNum,transact,amount,time)values('" + accNum + "','Deposit','" + value + "',CURRENT_TIMESTAMP)", con);
                            storedProcedure = "INSERT_PKG.insertNewTrans";
                            cmd.CommandText = storedProcedure;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("INaccNum", OracleDbType.Varchar2).Value = accNum;
                            cmd.Parameters.Add("transact", OracleDbType.Varchar2).Value = "D";
                            cmd.Parameters.Add("amount", OracleDbType.Varchar2).Value = value;
                            cmd.Parameters.Add("transfer", OracleDbType.Varchar2).Value = accNum;
                            cmd.ExecuteNonQuery();
                        }
                        //cmd = new OracleCommand("SELECT balance FROM info WHERE email='" + curUser + "'", con);
                        cmd = new OracleCommand();
                        cmd.Connection = con;
                        storedProcedure = "SELECT_PKG.getBalance";
                        cmd.CommandText = storedProcedure;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("curBalance", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
                        object newBal = cmd.ExecuteScalar();
                        //double cur;
                        if (newBal != null)
                        {
                            newBalance = Convert.ToDouble(newBal);
                            Console.WriteLine(newBalance);
                            balance.Text = "$" + newBalance;
                        }
                        con.Close();
                    }
                }
                dAmount.Text = "";
                refresh(sender, e);
            }
        }

        protected void withdrawAmount(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(wAmount.Text))
            {
                bool isNumeric = int.TryParse(wAmount.Text, out _);
                if (isNumeric)
                {
                    double value = Convert.ToDouble(wAmount.Text);
                    double cur;
                    double newBalance;
                    if (value > 0)
                    {
                        string ConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                        OracleConnection con = new OracleConnection(ConnectionString);
                        con.Open();
                        //OracleCommand cmd = new OracleCommand("SELECT balance FROM info WHERE email='" + curUser + "'", con);
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = con;
                        storedProcedure = "SELECT_PKG.getBalance";
                        cmd.CommandText = storedProcedure;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("curBalance", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
                        object bal = cmd.ExecuteScalar();
                        //double cur;
                        if (bal != null)
                        {
                            cur = Convert.ToDouble(bal);
                            Console.WriteLine(cur);
                            balance.Text = "$" + cur;
                            if (cur - value >= 0)
                            {
                                insufficientFundsW.Visible = false;
                                insufficientFundsW.Attributes.CssStyle.Add("display", "none");
                                //cmd = new OracleCommand("UPDATE info SET balance='" + (cur - value) + "' WHERE email='" + curUser + "'", con);
                                storedProcedure = "UPDATE_PKG.updateBalanceSub";
                                cmd.CommandText = storedProcedure;
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("subX", OracleDbType.Decimal).Value = cur;
                                cmd.Parameters.Add("subY", OracleDbType.Decimal).Value = value;
                                cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
                                cmd.ExecuteNonQuery();
                                //cmd = new OracleCommand("select accNum from info where email='" + curUser + "'", con);
                                storedProcedure = "SELECT_PKG.getAccNum";
                                cmd.CommandText = storedProcedure;
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("curAccNum", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                                cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
                                object readAccNum = cmd.ExecuteScalar();
                                accNum = Convert.ToInt32(readAccNum);
                                //cmd = new OracleCommand("insert into transactions(accNum,transact,amount,time)values('" + accNum + "','Withdraw','" + value + "',CURRENT_TIMESTAMP)", con);
                                storedProcedure = "INSERT_PKG.insertNewTrans";
                                cmd.CommandText = storedProcedure;
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("INaccNum", OracleDbType.Int64).Value = accNum;
                                cmd.Parameters.Add("transact", OracleDbType.Varchar2).Value = "W";
                                cmd.Parameters.Add("amount", OracleDbType.Decimal).Value = value;
                                cmd.Parameters.Add("transfer", OracleDbType.Int64).Value = accNum;
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                insufficientFundsW.Visible = true;
                                insufficientFundsW.Attributes.CssStyle.Add("display", "normal");
                            }
                        }
                        cmd = new OracleCommand("SELECT balance FROM info WHERE email='" + curUser + "'", con);
                        object newBal = cmd.ExecuteScalar();
                        //double cur;
                        if (newBal != null)
                        {
                            newBalance = Convert.ToDouble(newBal);
                            Console.WriteLine(newBalance);
                            balance.Text = "$" + newBalance;
                        }
                        con.Close();
                    }
                }
                wAmount.Text = "";
                refresh(sender, e);
            }
        }

        protected void sendMoneyToUser(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(sendTo.Text) && !String.IsNullOrWhiteSpace(sendAmount.Text))
            {
                if (IsValidEmailId(sendTo.Text) && DomainExists(sendTo.Text) && sendTo.Text != curUser)
                {
                    bool emailExists = false;
                    string ConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                    OracleConnection con = new OracleConnection(ConnectionString);
                    con.Open();
                    OracleDataReader dataReader;
                    OracleCommand cmd = new OracleCommand("select email from info", con);
                    dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine(dataReader.GetString(0));
                        if (dataReader.GetString(0) == sendTo.Text)
                        {
                            emailExists = true;
                        }
                    }
                    dataReader.Close();
                    if (emailExists)
                    {
                        bool isNumeric = int.TryParse(sendAmount.Text, out _);
                        if (isNumeric)
                        {
                            double value = Convert.ToDouble(sendAmount.Text);
                            double newBalance;
                            double cur;
                            double otherBal;
                            invalidUser.Visible = false;
                            invalidUser.Attributes.CssStyle.Add("display", "none");
                            //cmd = new OracleCommand("SELECT balance FROM info WHERE email='" + curUser + "'", con);
                            cmd = new OracleCommand();
                            cmd.Connection = con;
                            storedProcedure = "SELECT_PKG.getBalance";
                            cmd.CommandText = storedProcedure;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("curBalance", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
                            object bal = cmd.ExecuteScalar();
                            //cmd = new OracleCommand("SELECT balance FROM info WHERE email='" + sendTo.Text + "'", con);
                            cmd = new OracleCommand();
                            cmd.Connection = con;
                            storedProcedure = "SELECT_PKG.getBalance";
                            cmd.CommandText = storedProcedure;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("curBalance", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = sendTo.Text;
                            object otherUser = cmd.ExecuteScalar();
                            if (bal != null && otherUser != null)
                            {
                                cur = Convert.ToDouble(bal);
                                Console.WriteLine(cur);
                                otherBal = Convert.ToDouble(otherUser);
                                balance.Text = "$" + cur;
                                if (cur - value >= 0)
                                {
                                    insufficientFunds.Visible = false;
                                    insufficientFunds.Attributes.CssStyle.Add("display", "none");
                                    //cmd = new OracleCommand("UPDATE info SET balance='" + (cur - value) + "' WHERE email='" + curUser + "'", con);
                                    storedProcedure = "UPDATE_PKG.updateBalanceSub";
                                    cmd.CommandText = storedProcedure;
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("subX", OracleDbType.Decimal).Value = cur;
                                    cmd.Parameters.Add("subY", OracleDbType.Decimal).Value = value;
                                    cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
                                    cmd.ExecuteNonQuery();
                                    //cmd = new OracleCommand("UPDATE info SET balance='" + (otherBal + value) + "' WHERE email='" + sendTo.Text + "'", con);storedProcedure = "updateBalance";
                                    storedProcedure = "UPDATE_PKG.updateBalanceAdd";
                                    cmd.CommandText = storedProcedure;
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("addX", OracleDbType.Decimal).Value = otherBal;
                                    cmd.Parameters.Add("addY", OracleDbType.Decimal).Value = value;
                                    cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = sendTo.Text;
                                    cmd.ExecuteNonQuery();
                                    //cmd = new OracleCommand("select accNum from info where email='" + curUser + "'", con);
                                    storedProcedure = "SELECT_PKG.getAccNum";
                                    cmd.CommandText = storedProcedure;
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("curAccNum", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                                    cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
                                    object readAccNum = cmd.ExecuteScalar();
                                    accNum = Convert.ToInt32(readAccNum);
                                    //cmd = new OracleCommand("insert into transactions(accNum,transact,amount,time)values('" + accNum + "','Sent to "+sendTo.Text+"','" + value + "',CURRENT_TIMESTAMP)", con);
                                    storedProcedure = "SELECT_PKG.getAccNum";
                                    cmd.CommandText = storedProcedure;
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("curAccNum", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                                    cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = sendTo.Text;
                                    object receiverAccNum = cmd.ExecuteScalar();
                                    receiverAccNum = Convert.ToInt32(receiverAccNum);
                                    storedProcedure = "INSERT_PKG.insertNewTrans";
                                    cmd.CommandText = storedProcedure;
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("INaccNum", OracleDbType.Int64).Value = accNum;
                                    cmd.Parameters.Add("transact", OracleDbType.Varchar2).Value = "TOut";
                                    cmd.Parameters.Add("amount", OracleDbType.Decimal).Value = value;
                                    cmd.Parameters.Add("transfer", OracleDbType.Int64).Value = receiverAccNum;
                                    cmd.ExecuteNonQuery();
                                    //
                                    storedProcedure = "INSERT_PKG.insertNewTrans";
                                    cmd.CommandText = storedProcedure;
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("INaccNum", OracleDbType.Int64).Value = receiverAccNum;
                                    cmd.Parameters.Add("transact", OracleDbType.Varchar2).Value = "TIn";
                                    cmd.Parameters.Add("amount", OracleDbType.Decimal).Value = value;
                                    cmd.Parameters.Add("transfer", OracleDbType.Int64).Value = accNum;
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    insufficientFunds.Attributes.CssStyle.Add("display", "normal");
                                    insufficientFunds.Visible = true;
                                }
                            }
                            //cmd = new OracleCommand("SELECT balance FROM info WHERE email='" + curUser + "'", con);
                            cmd = new OracleCommand();
                            cmd.Connection = con;
                            storedProcedure = "SELECT_PKG.getBalance";
                            cmd.CommandText = storedProcedure;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("curBalance", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
                            object newBal = cmd.ExecuteScalar();
                            //double cur;
                            if (newBal != null)
                            {
                                newBalance = Convert.ToDouble(newBal);
                                Console.WriteLine(newBalance);
                                balance.Text = "$" + newBalance;
                            }
                        }
                        else
                        {
                            invalidUser.Attributes.CssStyle.Add("display", "normal");
                            invalidUser.Visible = true;
                        }
                        con.Close();
                    }
                }
            }
            sendTo.Text = "";
            sendAmount.Text = "";
            refresh(sender, e);
        }
















        protected void DataToHTML(DataTable displayTable,int accNum)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            OracleConnection con = new OracleConnection(ConnectionString);
            con.Open();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = con;
            displayTable.Columns["transact"].ColumnName = "Transactions";
            displayTable.Columns["amount"].ColumnName = "Amount";
            displayTable.Columns["time"].ColumnName = "Time";
            StringBuilder dT = new StringBuilder();
            dT.Append("<div style='overflow-y:auto;'>");
            dT.Append("<table id='dtHTML' border='1' runat='server'>");
            dT.Append("<thead>");
            dT.Append("<tr>");
            foreach (DataColumn tableCol in displayTable.Columns)
            {
                if (tableCol.ColumnName != "TRANSFER" && tableCol.ColumnName != "FILEPATH")
                {
                    dT.Append("<th>");
                    dT.Append(tableCol.ColumnName);
                    dT.Append("</th>");
                }
            }
            dT.Append("</tr>");
            dT.Append("</thead>");
            dT.Append("<tbody>");
            foreach (DataRow tableRow in displayTable.Rows)
            {
                dT.Append("<tr>");
                foreach (DataColumn matchCol in displayTable.Columns)
                {
                    if (matchCol.ColumnName != "TRANSFER" && matchCol.ColumnName != "FILEPATH")
                    {
                        dT.Append("<td>");
                        if (matchCol.ColumnName == "Transactions")
                        {
                            if (tableRow["Transactions"].ToString() == "D")
                            {
                                dT.Append("<img src=" + tableRow["filePath"].ToString() + " height=50px width=50px>");
                                dT.Append("Deposit");
                            }
                            if (tableRow["Transactions"].ToString() == "W")
                            {
                                dT.Append("<img src=" + tableRow["filePath"].ToString() + " height=50px width=50px>");
                                dT.Append("Withdraw");
                            }
                            if (tableRow["Transactions"].ToString() == "TOut")
                            {
                                cmd.CommandText = "SELECT_PKG.getFilePath";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("retFilePath", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                                cmd.Parameters.Add("INaccNum", OracleDbType.Int64).Value = accNum;
                                object filePath = cmd.ExecuteScalar().ToString();
                                dT.Append("<img src=" + filePath + " height =50px width=50px>");
                                storedProcedure = "SELECT_PKG.getEmail";
                                cmd.CommandText = storedProcedure;
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("retEmail", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                                cmd.Parameters.Add("INaccNum", OracleDbType.Int64).Value = Convert.ToInt32(tableRow["transfer"]);
                                object receiverEmail = cmd.ExecuteScalar();
                                receiverEmail = receiverEmail.ToString();
                                dT.Append("Sent to " + receiverEmail);
                            }
                            if (tableRow["Transactions"].ToString() == "TIn")
                            {
                                dT.Append("<img src=" + tableRow["filePath"].ToString() + " height=50px width=50px>");
                                storedProcedure = "SELECT_PKG.getEmail";
                                cmd.CommandText = storedProcedure;
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("retEmail", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                                cmd.Parameters.Add("INaccNum", OracleDbType.Int64).Value = Convert.ToInt32(tableRow["transfer"]);
                                object senderEmail = cmd.ExecuteScalar();
                                senderEmail = senderEmail.ToString();
                                dT.Append("Sent from " + senderEmail);
                            }
                        }
                        else if (matchCol.ColumnName == "Amount")
                        {
                            if (tableRow["Transactions"].ToString() == "D")
                            {
                                dT.Append("+" + Convert.ToDecimal(tableRow[matchCol]).ToString("C2"));
                            }
                            else if (tableRow["Transactions"].ToString() == "W")
                            {
                                dT.Append("-" + Convert.ToDecimal(tableRow[matchCol]).ToString("C2"));
                            }
                            else if (tableRow["Transactions"].ToString() == "TOut")
                            {
                                dT.Append("-" + Convert.ToDecimal(tableRow[matchCol]).ToString("C2"));
                            }
                            else if (tableRow["Transactions"].ToString() == "TIn")
                            {
                                dT.Append("+" + Convert.ToDecimal(tableRow[matchCol]).ToString("C2"));
                            }
                            else
                            {
                                dT.Append("-" + Convert.ToDecimal(tableRow[matchCol]).ToString("C2"));
                            }
                        }
                        else if (matchCol.ColumnName == "Time")
                        {
                            dT.Append(Convert.ToDateTime(tableRow[matchCol]).ToString("g"));
                        }
                        else
                        {
                            dT.Append(tableRow[matchCol].ToString());
                        }

                        dT.Append("</td>");
                    }
                }
                dT.Append("</tr>");
            }
            dT.Append("</tbody>");
            dT.Append("</table>");
            dT.Append("</div>");
            PlaceHolder.Controls.Clear();
            PlaceHolder.Controls.Add(new Literal { Text = dT.ToString() });
            con.Close();
        }

        protected void refresh(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            OracleConnection con = new OracleConnection(ConnectionString);
            con.Open();
            table = new StringBuilder();
            //OracleCommand cmd = new OracleCommand("select accNum from info where email='" + curUser + "'", con);
            storedProcedure = "SELECT_PKG.getAccNum";
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = con;
            cmd.CommandText = storedProcedure;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("curAccNum", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
            object readAccNum = cmd.ExecuteScalar();
            accNum = Convert.ToInt32(readAccNum);
            storedProcedure = "SELECT_PKG.showUsingPaging";
            //cmd = new OracleCommand("with selectRes as (select transactions.transact, transactions.amount, transactions.time from transactions where transactions.accNum = '"+accNum+"' union all select transfers.transact, transfers.amount, transfers.time from transfers where transfers.accNum = '"+accNum+"')select top 10 * from selectRes order by time desc", con);
            cmd.CommandText = storedProcedure;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("retData", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            if (Convert.ToInt64(Session["PageNum"]) == 0)
            {
                cmd.Parameters.Add("PageNumber", OracleDbType.Int64).Value = 1;
            }
            else
            {
                cmd.Parameters.Add("PageNumber", OracleDbType.Int64).Value = Convert.ToInt64(Session["PageNum"]);
            }
            //cmd.Parameters.Add("PageNumber", OracleDbType.Int).Value = pageNum;
            cmd.Parameters.Add("accNumber", OracleDbType.Int64).Value = accNum;
            holdMonth = Convert.ToString(Session["holdMonth"]);
            holdYear = Convert.ToString(Session["holdYear"]);
            cmd.Parameters.Add("month", OracleDbType.Varchar2).Value = holdMonth;
            cmd.Parameters.Add("year", OracleDbType.Varchar2).Value = holdYear;
            DataTable displayTable = new DataTable();
            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
            adapter.Fill(displayTable);
            DataToHTML(displayTable, accNum);
            pageNumber.InnerText = "" + Convert.ToString(Session["pageNum"]) + "/" + numPages + "";
            con.Close();
        }

        protected void viewAllTransactions(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            OracleConnection con = new OracleConnection(ConnectionString);
            con.Open();
            table = new StringBuilder();
            //OracleCommand cmd = new OracleCommand("select accNum from info where email='" + curUser + "'", con);
            storedProcedure = "SELECT_PKG.getAccNum";
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = con;
            cmd.CommandText = storedProcedure;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("curAccNum", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("curUser", OracleDbType.Varchar2).Value = curUser;
            object readAccNum = cmd.ExecuteScalar();
            accNum = Convert.ToInt32(readAccNum);
            //cmd = new OracleCommand("select transactions.transact, transactions.amount, transactions.time from transactions where transactions.accNum = '"+accNum+"' union all select transfers.transact, transfers.amount, transfers.time from transfers where transfers.accNum = '"+accNum+"' order by time desc", con);
            storedProcedure = "SELECT_PKG.showUsingPaging";
            cmd.CommandText = storedProcedure;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("retData", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("PageNumber", OracleDbType.Int64).Value = 1;
            cmd.Parameters.Add("accNumber", OracleDbType.Int64).Value = accNum;
            DateTime curDateTime = DateTime.Now;
            cmd.Parameters.Add("month", OracleDbType.Varchar2).Value = selectMonth.Value;
            cmd.Parameters.Add("year", OracleDbType.Varchar2).Value = selectYear.Value;
            DataTable fullTable = new DataTable();
            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
            adapter.Fill(fullTable);
            if (selectMonth.Value == "" || selectYear.Value=="")
            {
                noResults.Visible = false;
                selectError.Visible = true;
                refresh(sender, e);
            }
            else if (fullTable.Rows.Count==0)
            {
                noResults.Visible = true;
                selectError.Visible = false;
            }
            else
            {
                Session["holdMonth"] = selectMonth.Value;
                Session["holdYear"] = selectYear.Value;
                DataToHTML(fullTable,accNum);
                noResults.Visible = false;
                selectError.Visible = false;
            }
            con.Close();
        }

        protected void NextRecords(object sender, EventArgs e)
        {
            if (Convert.ToInt32(Session["pageNum"]) == 0)
            {
                Session["pageNum"] = 1;
            }
            if (Convert.ToInt32(Session["pageNum"]) < numPages)
            {
                pageNum = Convert.ToInt32(Session["pageNum"]);
                pageNum += 1;
                Session["pageNum"] = pageNum;
                refresh(sender, e);
            }
        }

        protected void PreviousRecords(object sender, EventArgs e)
        {
            pageNum = Convert.ToInt32(Session["pageNum"]);
            if (pageNum > 1)
            {
                pageNum -= 1;
                Session["pageNum"] = pageNum;
            }
            refresh(sender, e);
        }


        //HTML functions
        protected void Timer1_Tick(object sender, EventArgs e)
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

        protected void depositButton(object sender, EventArgs e)
        {
            dAmount.Visible = true;
            deposit.Visible = true;
            cancelD.Visible = true;
        }
        protected void withdrawButton(object sender, EventArgs e)
        {
            wAmount.Visible = true;
            withdraw.Visible = true;
            cancelW.Visible = true;
        }
        protected void sendButton(object sender, EventArgs e)
        {
            sendTo.Visible = true;
            sendAmount.Visible = true;
            sendMoney.Visible = true;
            cancelS.Visible = true;
        }

        protected void cancelDeposit(object sender, EventArgs e)
        {
            dAmount.Visible = false;
            deposit.Visible = false;
            cancelD.Visible = false;
        }
        protected void cancelWithdraw(object sender, EventArgs e)
        {
            wAmount.Visible = false;
            withdraw.Visible = false;
            cancelW.Visible = false;
            insufficientFundsW.Visible = false;
            insufficientFundsW.Attributes.CssStyle.Add("display", "none");
        }
        protected void cancelSend(object sender, EventArgs e)
        {
            sendTo.Visible = false;
            sendAmount.Visible = false;
            sendMoney.Visible = false;
            cancelS.Visible = false;
            invalidUser.Visible = false;
            invalidUser.Attributes.CssStyle.Add("display", "none");
            insufficientFunds.Visible = false;
            insufficientFunds.Attributes.CssStyle.Add("display", "none");
        }

        protected void logOut(object sender, EventArgs e)
        {
            Server.Transfer("SignIn.aspx");
        }
    }
}





/*OracleDataReader transChart = cmd.ExecuteReader();
            table.Append("<table id='DataTable' border='1'>");
            table.Append("<tr><th>Transaction</th><th>Amount</th><th>Time</th>");
            table.Append("</tr>");
            if (transChart.HasRows)
            {
                while (transChart.Read())
                {
                    table.Append("<tr>");
                    table.Append("<td>" + transChart[0] + "</td>");
                    if (transChart[0].ToString() == "Deposit" || transChart[0].ToString().Contains("Sent from"))
                    {
                        table.Append("<td>" + "+$" + transChart[1].ToString() + "</td>");
                    }
                    else if (transChart[0].ToString() == "Withdraw" || transChart[0].ToString().Contains("Sent to"))
                    {
                        table.Append("<td>" + "-$" + transChart[1].ToString() + "</td>");
                    }
                    table.Append("<td>" + transChart[2] + "</td>");
                    table.Append("</tr>");

                }
            }
            table.Append("</table>");
            PlaceHolder1.Controls.Add(new Literal { Text = table.ToString() });
            transChart.Close();*/