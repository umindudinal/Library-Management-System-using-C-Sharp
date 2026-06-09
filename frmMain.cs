using System;
using System.Windows.Forms;

namespace SarasaviLibrarySystem
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void btnBookReg_Click(object sender, EventArgs e)
        {
            frmBookReg frm = new frmBookReg();
            frm.Show();
        }

        private void btnUserReg_Click(object sender, EventArgs e)
        {
            frmUserReg frm = new frmUserReg();
            frm.Show();
        }

        private void btnLoan_Click(object sender, EventArgs e)
        {
            frmLoan frm = new frmLoan();
            frm.Show();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            frmReturn frm = new frmReturn();
            frm.Show();
        }

        private void btnReservation_Click(object sender, EventArgs e)
        {
            frmReservation frm = new frmReservation();
            frm.Show();
        }

        private void btnInquiry_Click(object sender, EventArgs e)
        {
            frmInquiry frm = new frmInquiry();
            frm.Show();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                frmLogin login = new frmLogin();
                login.Show();
                this.Close();
            }
        }
    }
}