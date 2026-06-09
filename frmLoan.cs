using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SarasaviLibrarySystem
{
    public partial class frmLoan : Form
    {
        public frmLoan()
        {
            InitializeComponent();
        }

        private void frmLoan_Load(object sender, EventArgs e)
        {
            txtLoanDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtReturnDate.Text = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd");
        }

        private void btnSearchUser_Click(object sender, EventArgs e)
        {
            if (txtUserNumber.Text.Trim() == "")
            {
                MessageBox.Show("Please enter User Number!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Check user exists and is a Member
                    string query = "SELECT Name, UserType FROM Users WHERE UserNumber=@userno";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userno", txtUserNumber.Text.Trim());
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        string userType = reader["UserType"].ToString();

                        if (userType == "Visitor")
                        {
                            MessageBox.Show("Visitors cannot borrow books!", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtUserName.Clear();
                            return;
                        }

                        txtUserName.Text = reader["Name"].ToString();
                        reader.Close();

                        // Check active loans count
                        string loanQuery = "SELECT COUNT(*) FROM Loans WHERE UserNumber=@userno AND Status='Active'";
                        SqlCommand loanCmd = new SqlCommand(loanQuery, conn);
                        loanCmd.Parameters.AddWithValue("@userno", txtUserNumber.Text.Trim());
                        int activeLoans = (int)loanCmd.ExecuteScalar();

                        if (activeLoans >= 5)
                        {
                            MessageBox.Show("This user has reached maximum loan limit (5 books)!", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtUserName.Clear();
                        }
                    }
                    else
                    {
                        MessageBox.Show("User not found!", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtUserName.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearchCopy_Click(object sender, EventArgs e)
        {
            if (txtCopyNumber.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Copy Number!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT c.Status, b.Title 
                                    FROM Copies c 
                                    INNER JOIN Books b ON c.BookNumber = b.BookNumber 
                                    WHERE c.CopyNumber=@copyno";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@copyno", txtCopyNumber.Text.Trim());
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        string status = reader["Status"].ToString();

                        if (status == "Reference")
                        {
                            MessageBox.Show("This copy is for Reference only — cannot be borrowed!", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtBookTitle.Clear();
                            return;
                        }

                        txtBookTitle.Text = reader["Title"].ToString();
                        reader.Close();

                        // Check if already loaned
                        string loanCheck = "SELECT COUNT(*) FROM Loans WHERE CopyNumber=@copyno AND Status='Active'";
                        SqlCommand loanCmd = new SqlCommand(loanCheck, conn);
                        loanCmd.Parameters.AddWithValue("@copyno", txtCopyNumber.Text.Trim());
                        int isLoaned = (int)loanCmd.ExecuteScalar();

                        if (isLoaned > 0)
                        {
                            MessageBox.Show("This copy is already on loan!", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtBookTitle.Clear();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Copy not found!", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtBookTitle.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtUserName.Text.Trim() == "" || txtBookTitle.Text.Trim() == "")
            {
                MessageBox.Show("Please search User and Copy first!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO Loans (CopyNumber, UserNumber, LoanDate, ExpectedReturnDate, Status) 
                                    VALUES (@copyno, @userno, @loandate, @returndate, 'Active')";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@copyno", txtCopyNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@userno", txtUserNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@loandate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@returndate", DateTime.Now.AddDays(14));
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Loan Saved Successfully!\nReturn Date: " + txtReturnDate.Text, "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtUserNumber.Clear();
            txtUserName.Clear();
            txtCopyNumber.Clear();
            txtBookTitle.Clear();
            txtLoanDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtReturnDate.Text = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd");
            txtUserNumber.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}