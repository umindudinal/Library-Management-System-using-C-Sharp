using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SarasaviLibrarySystem
{
    public partial class frmReturn : Form
    {
        private int activeLoanID = 0;

        public frmReturn()
        {
            InitializeComponent();
        }

        private void frmReturn_Load(object sender, EventArgs e)
        {
            txtReturnDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void btnSearch_Click(object sender, EventArgs e)
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
                    string query = @"SELECT l.LoanID, b.Title, u.Name, 
                                    l.LoanDate, l.ExpectedReturnDate
                                    FROM Loans l
                                    INNER JOIN Copies c ON l.CopyNumber = c.CopyNumber
                                    INNER JOIN Books b ON c.BookNumber = b.BookNumber
                                    INNER JOIN Users u ON l.UserNumber = u.UserNumber
                                    WHERE l.CopyNumber=@copyno AND l.Status='Active'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@copyno", txtCopyNumber.Text.Trim());
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        activeLoanID = (int)reader["LoanID"];
                        txtBookTitle.Text = reader["Title"].ToString();
                        txtUserName.Text = reader["Name"].ToString();
                        txtLoanDate.Text = Convert.ToDateTime(reader["LoanDate"]).ToString("yyyy-MM-dd");
                        txtExpectedReturn.Text = Convert.ToDateTime(reader["ExpectedReturnDate"]).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        MessageBox.Show("No active loan found for this Copy Number!", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClearFields();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            if (activeLoanID == 0)
            {
                MessageBox.Show("Please search a Copy first!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Update Loan Status
                    string query = @"UPDATE Loans SET Status='Returned', 
                                    ActualReturnDate=@returndate 
                                    WHERE LoanID=@loanid";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@returndate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@loanid", activeLoanID);
                    cmd.ExecuteNonQuery();

                    // Check Reservations for this book
                    string copyNo = txtCopyNumber.Text.Trim();
                    string bookQuery = @"SELECT b.BookNumber, b.Title 
                                        FROM Copies c 
                                        INNER JOIN Books b ON c.BookNumber = b.BookNumber 
                                        WHERE c.CopyNumber=@copyno";
                    SqlCommand bookCmd = new SqlCommand(bookQuery, conn);
                    bookCmd.Parameters.AddWithValue("@copyno", copyNo);
                    SqlDataReader bookReader = bookCmd.ExecuteReader();

                    if (bookReader.HasRows)
                    {
                        bookReader.Read();
                        string bookNumber = bookReader["BookNumber"].ToString();
                        string bookTitle = bookReader["Title"].ToString();
                        bookReader.Close();

                        // Check oldest reservation
                        string resQuery = @"SELECT TOP 1 r.ReservationID, u.Name 
                                           FROM Reservations r
                                           INNER JOIN Users u ON r.UserNumber = u.UserNumber
                                           WHERE r.BookNumber=@bookno AND r.Status='Pending'
                                           ORDER BY r.ReservationDate ASC";
                        SqlCommand resCmd = new SqlCommand(resQuery, conn);
                        resCmd.Parameters.AddWithValue("@bookno", bookNumber);
                        SqlDataReader resReader = resCmd.ExecuteReader();

                        if (resReader.HasRows)
                        {
                            resReader.Read();
                            int resID = (int)resReader["ReservationID"];
                            string memberName = resReader["Name"].ToString();
                            resReader.Close();

                            MessageBox.Show($"Book '{bookTitle}' is reserved by {memberName}!\nPlease set aside this copy.",
                                "Reservation Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Delete oldest reservation
                            string deleteRes = "UPDATE Reservations SET Status='Completed' WHERE ReservationID=@resid";
                            SqlCommand deleteCmd = new SqlCommand(deleteRes, conn);
                            deleteCmd.Parameters.AddWithValue("@resid", resID);
                            deleteCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            resReader.Close();
                        }
                    }

                    MessageBox.Show("Book Returned Successfully!", "Success",
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
            activeLoanID = 0;
            txtCopyNumber.Clear();
            txtBookTitle.Clear();
            txtUserName.Clear();
            txtLoanDate.Clear();
            txtExpectedReturn.Clear();
            txtReturnDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtCopyNumber.Focus();
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