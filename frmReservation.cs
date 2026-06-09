using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SarasaviLibrarySystem
{
    public partial class frmReservation : Form
    {
        public frmReservation()
        {
            InitializeComponent();
        }

        private void frmReservation_Load(object sender, EventArgs e)
        {
            txtReservationDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
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
                            MessageBox.Show("Visitors cannot make reservations!", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtUserName.Clear();
                            return;
                        }

                        txtUserName.Text = reader["Name"].ToString();
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

        private void btnSearchBook_Click(object sender, EventArgs e)
        {
            if (txtBookNumber.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Book Number!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT Title FROM Books WHERE BookNumber=@bookno";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@bookno", txtBookNumber.Text.Trim());
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        txtBookTitle.Text = reader["Title"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Book not found!", "Error",
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
                MessageBox.Show("Please search User and Book first!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Check already reserved by same user
                    string checkQuery = @"SELECT COUNT(*) FROM Reservations 
                                        WHERE BookNumber=@bookno 
                                        AND UserNumber=@userno 
                                        AND Status='Pending'";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@bookno", txtBookNumber.Text.Trim());
                    checkCmd.Parameters.AddWithValue("@userno", txtUserNumber.Text.Trim());
                    int existing = (int)checkCmd.ExecuteScalar();

                    if (existing > 0)
                    {
                        MessageBox.Show("This user has already reserved this book!", "Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Save Reservation
                    string query = @"INSERT INTO Reservations 
                                    (BookNumber, UserNumber, ReservationDate, Status) 
                                    VALUES (@bookno, @userno, @date, 'Pending')";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@bookno", txtBookNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@userno", txtUserNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Reservation Saved Successfully!", "Success",
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
            txtBookNumber.Clear();
            txtBookTitle.Clear();
            txtReservationDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
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