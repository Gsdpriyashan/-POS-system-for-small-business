using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace POS
{
    public partial class Payment : Form
    {
        private decimal totalAmount;
        public Payment(decimal total)
        {
            InitializeComponent();
            totalAmount = total;
        }

        private void Payment_Load(object sender, EventArgs e)
        {
            tot.Text = totalAmount.ToString("F2");
        }

        private void qty_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lt.Text))
                {
                    return;
                }
                var reader = new db().Select($"SELECT * FROM Customer WHERE Phone LIKE '%{lt.Text}%'");
                bool hasValidData = false;
                while (reader.Read())
                {
                    if (!hasValidData)
                    {
                        Status.Text = "Active";
                        hasValidData = true;
                    }
                }

                if (!hasValidData)
                {
                    Status.Text = "Not Active";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string phoneNumber = lt.Text;
                string customerID = null;
                string fetchCustomerIDQuery = $"SELECT CustomerID FROM Customer WHERE Phone = '{phoneNumber}'";

                db database = new db();
                var reader = database.Select(fetchCustomerIDQuery);

                if (reader != null && reader.Read())
                {
                    customerID = reader["CustomerID"].ToString();
                }
                reader.Close();

                if (string.IsNullOrEmpty(customerID))
                {
                    MessageBox.Show("Customer not found!");
                    return;
                }

                // Proceed with the sale recording
                var date = DateTime.Now;
                var query = $"INSERT INTO Sales (CustomerID, SaleDate, TotalAmount) VALUES ('{customerID}', '{date}', '{tot.Text}')";

                database.Execute(query);

                MessageBox.Show("Sale recorded successfully!");
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"SQL Error: {sqlEx.Message}");
            }
            catch (FormatException formatEx)
            {
                MessageBox.Show($"Format Error: {formatEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }

    }
}