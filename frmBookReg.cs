using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SarasaviLibrarySystem
{
    public partial class frmBookReg : Form
    {
        public frmBookReg()
        {
            InitializeComponent();
        }

        private void frmBookReg_Load(object sender, EventArgs e)
        {
            cmbStatus.Items.Add("Borrowable");
            cmbStatus.Items.Add("Reference");
            cmbStatus.SelectedIndex = 0;

            GenerateBookNumber();
        }

        private void GenerateBookNumber()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT TOP 1 BookNumber FROM Books WHERE Classification=@cls ORDER BY BookNumber DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@cls", txtClassification.Text.Trim().ToUpper());

                    object result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        txtBookNumber.Text = txtClassification.Text.Trim().ToUpper() + "0001";
                    }
                    else
                    {
                        string last = result.ToString();
                        int num = int.Parse(last.Substring(1)) + 1;
                        txtBookNumber.Text = txtClassification.Text.Trim().ToUpper() + num.ToString("D4");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void txtClassification_Leave(object sender, EventArgs e)
        {
            if (txtClassification.Text.Trim() != "")
            {
                GenerateBookNumber();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtClassification.Text.Trim() == "" || txtTitle.Text.Trim() == "" ||
                txtAuthor.Text.Trim() == "")
            {
                MessageBox.Show("Please fill Classification, Title and Author!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Save Book
                    string bookQuery = "INSERT INTO Books VALUES (@bookno, @title, @author, @publisher, @cls, @isbn)";
                    SqlCommand bookCmd = new SqlCommand(bookQuery, conn);
                    bookCmd.Parameters.AddWithValue("@bookno", txtBookNumber.Text);
                    bookCmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                    bookCmd.Parameters.AddWithValue("@author", txtAuthor.Text.Trim());
                    bookCmd.Parameters.AddWithValue("@publisher", txtPublisher.Text.Trim());
                    bookCmd.Parameters.AddWithValue("@cls", txtClassification.Text.Trim().ToUpper());
                    bookCmd.Parameters.AddWithValue("@isbn", txtISBN.Text.Trim());
                    bookCmd.ExecuteNonQuery();

                    // Save Copy
                    string copyNo = txtBookNumber.Text + "1";
                    string copyQuery = "INSERT INTO Copies VALUES (@copyno, @bookno, @status)";
                    SqlCommand copyCmd = new SqlCommand(copyQuery, conn);
                    copyCmd.Parameters.AddWithValue("@copyno", copyNo);
                    copyCmd.Parameters.AddWithValue("@bookno", txtBookNumber.Text);
                    copyCmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem.ToString());
                    copyCmd.ExecuteNonQuery();

                    MessageBox.Show("Book Registered Successfully!", "Success",
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
            txtClassification.Clear();
            txtTitle.Clear();
            txtAuthor.Clear();
            txtPublisher.Clear();
            txtISBN.Clear();
            txtBookNumber.Clear();
            cmbStatus.SelectedIndex = 0;
            txtClassification.Focus();
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