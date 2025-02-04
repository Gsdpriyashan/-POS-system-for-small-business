using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Xml.Linq;

namespace POS
{
    public partial class product : UserControl
    {
        public product()
        {
            InitializeComponent();
        }
        private void viewdetails()
        {
            var reader = new db().Select("SELECT * FROM Product");
            dataGridView1.Rows.Clear();
            while (reader.Read())
            {
                dataGridView1.Rows.Add(reader["ProductID"], reader["Name"], reader["Price"], reader["Category"]);
            }
        }

        private void ClearFields()
        {
            pname.Text = "";
            pprice.Text = "";
            pcat.Text = "";
        }
        private void product_Load(object sender, EventArgs e)
        {
            viewdetails();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string PNAME = pname.Text.Trim();
            string PPRICE = pprice.Text.Trim();
            string PCATEGORY = pcat.Text.Trim();

            db DB = new db();

            string query = $@"INSERT INTO Product (Name, Price , Category) 
            VALUES ('{PNAME}' , '{PPRICE}' ,'{PCATEGORY}')";

            var reader = new db().Select($"SELECT * FROM Product WHERE  Name = '{PNAME}'");
            var result = reader.Read();
            if (result)
            {
                MessageBox.Show("Product already exists.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
                var reader = new db().Select($"SELECT * FROM Product WHERE ProductID LIKE '%{Search.Text}%'");
                bool hasValidData = false;
                while (reader.Read())
                {
                    var PID = reader["ProductID"]?.ToString();
                    var name = reader["Name"]?.ToString();
                    var price = reader["Price"]?.ToString();
                    var cat = reader["Category"]?.ToString();

                    if (!string.IsNullOrWhiteSpace(PID) && !string.IsNullOrWhiteSpace(name))
                    {
                        dataGridView1.Rows.Add(PID, name, price, cat);

                        if (!hasValidData)
                        {
                            pname.Text = name;
                            pprice.Text = price ?? string.Empty;
                            pcat.Text = cat ?? string.Empty;
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
            string Pid = Search.Text.Trim();
            if (Pid == "")
            {
                MessageBox.Show("Please fill all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            db DB = new db();

            string query1 = $@"DELETE FROM Product WHERE ProductID = '{Pid}'";
            string query2 = $@"DELETE FROM Stock WHERE ProductID = '{Pid}'";
            string query3 = $@"DELETE FROM SalesDetail WHERE ProductID = '{Pid}'";
            string query4 = $@"DELETE FROM Discount WHERE ProductID = '{Pid}'";

            try
            {
                DB.Execute(query4);
                DB.Execute(query3);
                DB.Execute(query2);
                DB.Execute(query1);
                MessageBox.Show("Data deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                viewdetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string PID = Search.Text.Trim();
            string PNAME = pname.Text.Trim();
            string PPRICE = pprice.Text.Trim();
            string PCATEGORY = pcat.Text.Trim();

            db DB = new db();

            string query = $@"UPDATE Product SET Name = '{PNAME}', Price = '{PPRICE}', Category = '{PCATEGORY}' WHERE ProductID = '{PID}'";

            try
            {
                DB.Execute(query);
                MessageBox.Show("Data updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
