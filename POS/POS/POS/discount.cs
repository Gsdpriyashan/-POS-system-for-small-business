using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    public partial class discount : UserControl
    {
        public discount()
        {
            InitializeComponent();
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

        private void discount_Load(object sender, EventArgs e)
        {
            LoadProductID();
            viewdetails();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string PID = comboBox1.Text.Trim();
            string PVALUE = pvalue.Text.Trim();
            DateTime SDATE = sdate.Value;
            DateTime EDATE = edate.Value;

            db DB = new db();

            string query = $@"INSERT INTO Discount (ProductID, DiscountPercent , StartDate , EndDate) 
            VALUES ('{PID}' , '{PVALUE}' ,'{SDATE:yyyy-MM-dd}' ,'{EDATE:yyyy-MM-dd}')";


            try
            {
                // Execute SQL query
                DB.Execute(query);
                MessageBox.Show("Data inserted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                viewdetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearFields()
        {
            pvalue.Text = "";
            sdate.Text = "";
            edate.Text = "";
        }

        private void viewdetails()
        {
            var reader = new db().Select("SELECT * FROM Discount");
            dataGridView1.Rows.Clear();
            while (reader.Read())
            {
                dataGridView1.Rows.Add(reader["DiscountID"], reader["ProductID"], reader["DiscountPercent"], reader["StartDate"], reader["EndDate"]);
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
                    viewdetails();
                    return;
                }
                var reader = new db().Select($"SELECT * FROM Discount WHERE DiscountID LIKE '%{Search.Text}%'");
                bool hasValidData = false;
                while (reader.Read())
                {
                    var DID = reader["DiscountID"]?.ToString();
                    var PID = reader["ProductID"]?.ToString();
                    var pr = reader["DiscountPercent"]?.ToString();
                    var sdateValue = reader["StartDate"]?.ToString();
                    var edateValue = reader["EndDate"]?.ToString();

                    if (!string.IsNullOrWhiteSpace(DID) && !string.IsNullOrWhiteSpace(PID))
                    {
                        dataGridView1.Rows.Add(DID, PID, pr, sdateValue, edateValue);

                        if (!hasValidData)
                        {
                            comboBox1.Text = PID;
                            pvalue.Text = pr;
                            sdate.Text = sdateValue;
                            edate.Text = edateValue;
                            hasValidData = true;
                        }
                    }
                }

                if (!hasValidData)
                {
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string DID = Search.Text.Trim();
            if (DID == "")
            {
                MessageBox.Show("Please enter Discount ID to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            db DB = new db();
            string query = $"DELETE FROM Discount WHERE DiscountID = '{DID}'";

            try
            {
                // Execute SQL query
                DB.Execute(query);
                MessageBox.Show("Data deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                viewdetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
