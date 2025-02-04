using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    public partial class stock : UserControl
    {
        public stock()
        {
            InitializeComponent();
        }

        private void viewdetails()
        {
            var reader = new db().Select("SELECT * FROM Stock");
            dataGridView1.Rows.Clear();
            while (reader.Read())
            {
                dataGridView1.Rows.Add(reader["StockID"], reader["ProductID"], reader["Quantity"], reader["LastUpdated"]);
            }
        }
        private void stock_Load(object sender, EventArgs e)
        {
            LoadProductID();
            viewdetails();
        }

        private void LoadProductID()
        {
            try
            {
                db db = new db();

                using (SqlDataReader reader = db.Select("SELECT ProductID FROM Product"))
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    comboBox1.DisplayMember = "ProductID";
                    comboBox1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Employee data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                db db = new db();
                using (SqlDataReader reader = db.Select($"SELECT Name FROM Product WHERE ProductID = {comboBox1.Text}"))
                {
                    if (reader.Read())
                    {
                        label5.Text = reader["Name"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Employee data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                db db = new db();
                string query = $"INSERT INTO Stock (ProductID, Quantity, LastUpdated) VALUES ('{comboBox1.Text}', '{qty.Text}', '{DateTime.Now}')";
                db.Execute(query);
                MessageBox.Show("Data Inserted Successfully");
                viewdetails();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearFields()
        {
            qty.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                db db = new db();
                string query = $"UPDATE Stock SET Quantity = '{qty.Text}', LastUpdated = '{DateTime.Now}' WHERE StockID = '{Search.Text}'";
                db.Execute(query);
                MessageBox.Show("Data Updated Successfully");
                viewdetails();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Search_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Rows.Clear();

                if (string.IsNullOrWhiteSpace(Search.Text))
                {
                    ClearFields();
                    label5.Text = "NaN";
                    viewdetails();
                    return;
                }
                var reader = new db().Select($"SELECT * FROM Stock WHERE StockID LIKE '%{Search.Text}%'");
                bool hasValidData = false;
                while (reader.Read())
                {
                    var SID = reader["StockID"]?.ToString();
                    var PID = reader["ProductID"]?.ToString();
                    var QTY = reader["Quantity"]?.ToString();

                    if (!string.IsNullOrWhiteSpace(SID) && !string.IsNullOrWhiteSpace(PID))
                    {
                        dataGridView1.Rows.Add(SID, PID, QTY);

                        if (!hasValidData)
                        {
                            label1.Text = "NaN";
                            qty.Text = QTY ?? string.Empty;
                            hasValidData = true;
                        }
                    }
                }

                if (!hasValidData)
                {
                    ClearFields();
                    label5.Text = "NaN";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string Sid = Search.Text.Trim();
            if (Sid == "")
            {
                MessageBox.Show("Please Select StockID to Delete");
            }
            else
            {
                try
                {
                    db db = new db();
                    string query = $"DELETE FROM Stock WHERE StockID = '{Search.Text}'";
                    db.Execute(query);
                    MessageBox.Show("Data Deleted Successfully");
                    viewdetails();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
