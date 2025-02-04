using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    public partial class home : Form
    {
        public home()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            MainPNL.Controls.Clear();
            MainPNL.Controls.Add(new sale());
        }

        private void label3_Click(object sender, EventArgs e)
        {
            MainPNL.Controls.Clear();
            MainPNL.Controls.Add(new customer());
        }

        private void label2_Click(object sender, EventArgs e)
        {
            MainPNL.Controls.Clear();
            MainPNL.Controls.Add(new product());
        }

        private void label4_Click(object sender, EventArgs e)
        {
            MainPNL.Controls.Clear();
            MainPNL.Controls.Add(new stock());
        }

        private void label5_Click(object sender, EventArgs e)
        {
            MainPNL.Controls.Clear();
            MainPNL.Controls.Add(new discount());
        }

        private void label6_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to log out?", "Log Out", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                this.Hide();
                login login = new login();
                login.Show();
            }
        }
    }
}
