using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SarasaviLibrarySystem
{
    public partial class frmInquiry : Form
    {
        public frmInquiry()
        {
            InitializeComponent();
        }

        private void frmInquiry_Load(object sender, EventArgs e)
        {
            cmbSearchBy.Items.Add("Book Number");
            cmbSearchBy.Items.Add("Title");
            cmbSearchBy.Items.Add("Author");
            cmbSearchBy.SelectedIndex = 0;

            // DataGridView Setup
            dgvResults.ReadOnly = true;
            dgvResults.AllowUserToAddRows = false;
            dgvResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Search Text!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "";

                    if (cmbSearchBy.SelectedItem.ToString() == "Book Number")
                    {
                        query = @"SELECT b.BookNumber, b.Title, b.Author, b.Publisher,
                                 c.CopyNumber, c.Status as CopyStatus,
                                 CASE 
                                    WHEN l.Status = 'Active' THEN 'On Loan'
                                    WHEN r.Status = 'Pending' THEN 'Reserved'
                                    ELSE 'Available'
                                 END as Availability
                                 FROM Books b
                                 INNER JOIN Copies c ON b.BookNumber = c.BookNumber
                                 LEFT JOIN Loans l ON c.CopyNumber = l.CopyNumber AND l.Status = 'Active'
                                 LEFT JOIN Reservations r ON b.BookNumber = r.BookNumber AND r.Status = 'Pending'
                                 WHERE b.BookNumber = @search";
                    }
                    else if (cmbSearchBy.SelectedItem.ToString() == "Title")
                    {
                        query = @"SELECT b.BookNumber, b.Title, b.Author, b.Publisher,
                                 c.CopyNumber, c.Status as CopyStatus,
                                 CASE 
                                    WHEN l.Status = 'Active' THEN 'On Loan'
                                    WHEN r.Status = 'Pending' THEN 'Reserved'
                                    ELSE 'Available'
                                 END as Availability
                                 FROM Books b
                                 INNER JOIN Copies c ON b.BookNumber = c.BookNumber
                                 LEFT JOIN Loans l ON c.CopyNumber = l.CopyNumber AND l.Status = 'Active'
                                 LEFT JOIN Reservations r ON b.BookNumber = r.BookNumber AND r.Status = 'Pending'
                                 WHERE b.Title LIKE @search";
                    }
                    else
                    {
                        query = @"SELECT b.BookNumber, b.Title, b.Author, b.Publisher,
                                 c.CopyNumber, c.Status as CopyStatus,
                                 CASE 
                                    WHEN l.Status = 'Active' THEN 'On Loan'
                                    WHEN r.Status = 'Pending' THEN 'Reserved'
                                    ELSE 'Available'
                                 END as Availability
                                 FROM Books b
                                 INNER JOIN Copies c ON b.BookNumber = c.BookNumber
                                 LEFT JOIN Loans l ON c.CopyNumber = l.CopyNumber AND l.Status = 'Active'
                                 LEFT JOIN Reservations r ON b.BookNumber = r.BookNumber AND r.Status = 'Pending'
                                 WHERE b.Author LIKE @search";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (cmbSearchBy.SelectedItem.ToString() == "Book Number")
                        cmd.Parameters.AddWithValue("@search", txtSearch.Text.Trim());
                    else
                        cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text.Trim() + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        dgvResults.DataSource = dt;
                        txtStatus.Text = dt.Rows.Count + " record(s) found.";
                    }
                    else
                    {
                        dgvResults.DataSource = null;
                        txtStatus.Text = "No records found.";
                        MessageBox.Show("No books found!", "Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
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
            txtSearch.Clear();
            txtStatus.Clear();
            dgvResults.DataSource = null;
            cmbSearchBy.SelectedIndex = 0;
            txtSearch.Focus();
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