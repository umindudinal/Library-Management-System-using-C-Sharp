using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SarasaviLibrarySystem
{
    public partial class frmUserReg : Form
    {
        public frmUserReg()
        {
            InitializeComponent();
        }

        private void frmUserReg_Load(object sender, EventArgs e)
        {
            cmbSex.Items.Add("Male");
            cmbSex.Items.Add("Female");
            cmbSex.SelectedIndex = 0;

            cmbUserType.Items.Add("Member");
            cmbUserType.Items.Add("Visitor");
            cmbUserType.SelectedIndex = 0;

            GenerateUserNumber();
        }

        private void GenerateUserNumber()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Users";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    int count = (int)cmd.ExecuteScalar();
                    txtUserNumber.Text = "U" + (count + 1).ToString("D4");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "" || txtNIC.Text.Trim() == "")
            {
                MessageBox.Show("Please fill Name and NIC!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Users VALUES (@userno, @name, @sex, @nic, @address, @usertype)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userno", txtUserNumber.Text);
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@sex", cmbSex.SelectedItem.ToString()[0].ToString());
                    cmd.Parameters.AddWithValue("@nic", txtNIC.Text.Trim());
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@usertype", cmbUserType.SelectedItem.ToString());
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("User Registered Successfully!", "Success",
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
            txtName.Clear();
            txtNIC.Clear();
            txtAddress.Clear();
            cmbSex.SelectedIndex = 0;
            cmbUserType.SelectedIndex = 0;
            GenerateUserNumber();
            txtName.Focus();
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