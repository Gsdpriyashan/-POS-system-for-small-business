using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace POS
{
    public partial class customer : UserControl
    {
        public customer()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string CNAME = cname.Text.Trim();
            string CEMAIL = cemail.Text.Trim();
            string CPHONE = cphone.Text.Trim();
            string CADDRESS = caddress.Text.Trim();

            if (CNAME == "" || CEMAIL == "" || CPHONE == "" || CADDRESS == "")
            {
                MessageBox.Show("Please fill all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!CEMAIL.Contains("@") || !CEMAIL.Contains("."))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            db DB = new db();

            string query = $@"INSERT INTO Customer (Name, Email , Phone ,Address) 
            VALUES ('{CNAME}' , '{CEMAIL}' ,'{CPHONE}' , '{CADDRESS}')";

            var reader = new db().Select($"SELECT * FROM Customer WHERE  Phone = '{CPHONE}'");
            var result = reader.Read();
            if (result)
            {
                MessageBox.Show("Customer already exists.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void ClearFields()
        {
            cname.Text = "";
            cemail.Text = "";
            cphone.Text = "";
            caddress.Text = "";
        }

        private void viewdetails()
        {
            var reader = new db().Select("SELECT * FROM Customer");
            dataGridView1.Rows.Clear();
            while (reader.Read())
            {
                dataGridView1.Rows.Add(reader["CustomerID"], reader["Name"], reader["Email"], reader["Phone"], reader["Address"]);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void customer_Load(object sender, EventArgs e)
        {
            viewdetails();
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
                var reader = new db().Select($"SELECT * FROM Customer WHERE Name LIKE '%{Search.Text}%'");
                bool hasValidData = false;
                while (reader.Read())
                {
                    var customerID = reader["CustomerID"]?.ToString();
                    var name = reader["Name"]?.ToString();
                    var email = reader["Email"]?.ToString();
                    var phone = reader["Phone"]?.ToString();
                    var address = reader["Address"]?.ToString();

                    if (!string.IsNullOrWhiteSpace(customerID) && !string.IsNullOrWhiteSpace(name))
                    {
                        dataGridView1.Rows.Add(customerID, name, email, phone, address);

                        if (!hasValidData)
                        {
                            cname.Text = name;
                            cemail.Text = email ?? string.Empty;
                            cphone.Text = phone ?? string.Empty;
                            caddress.Text = address ?? string.Empty;

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

        private void button2_Click(object sender, EventArgs e)
        {
            string CNAME = cname.Text.Trim();
            string CEMAIL = cemail.Text.Trim();
            string CPHONE = cphone.Text.Trim();
            string CADDRESS = caddress.Text.Trim();

            if (CNAME == "" || CEMAIL == "" || CPHONE == "" || CADDRESS == "")
            {
                MessageBox.Show("Please fill all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!CEMAIL.Contains("@") || !CEMAIL.Contains("."))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            db DB = new db();

            string query = $@"UPDATE Customer SET Name = '{CNAME}', Email = '{CEMAIL}', Phone = '{CPHONE}', Address = '{CADDRESS}' WHERE Phone = '{CPHONE}'";

            try
            {
                // Execute SQL query
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

        private void button3_Click(object sender, EventArgs e)
        {
            string CPHONE = cphone.Text.Trim();
            if (CPHONE == "")
            {
                MessageBox.Show("Please fill all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            db DB = new db();

            // Execute the query to get the Customer record based on the search text
            var reader = new db().Select($"SELECT * FROM Customer WHERE Name LIKE '%{Search.Text}%'");

            // If the reader has data, proceed with the delete
            if (reader.HasRows)
            {
                reader.Read();  // Read the first matching record

                // Extract the CustomerID from the reader
                int customerID = Convert.ToInt32(reader["CustomerID"]); // Assuming CustomerID is an int

                // Define the SQL queries
                // 1. Delete from Invoice table based on SaleID (found via CustomerID in Sales)
                string query4 = $@"DELETE FROM Invoice WHERE SaleID IN (SELECT SaleID FROM Sales WHERE CustomerID = {customerID})";

                // 2. Delete from SaleDetails based on SaleID (found via CustomerID in Sales)
                string query3 = $@"DELETE FROM SalesDetail WHERE SaleID IN (SELECT SaleID FROM Sales WHERE CustomerID = {customerID})";

                // 3. Delete from Sales based on CustomerID
                string query2 = $@"DELETE FROM Sales WHERE CustomerID = {customerID}";

                // 4. Delete from Customer based on Phone
                string query1 = $@"DELETE FROM Customer WHERE Phone = '{CPHONE}'";

                try
                {
                    // Start by deleting from Invoice
                    DB.Execute(query4);

                    // Then delete from SaleDetails
                    DB.Execute(query3);

                    // Delete from Sales table
                    DB.Execute(query2);

                    // Finally, delete from Customer table
                    DB.Execute(query1);

                    // Success message
                    MessageBox.Show("Data deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear fields and refresh the view
                    ClearFields();
                    viewdetails();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No customer found with the provided name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
