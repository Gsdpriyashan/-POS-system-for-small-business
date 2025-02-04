using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    public partial class sale : UserControl
    {
        public sale()
        {
            InitializeComponent();
        }

        private void LoadProductID()
        {
            try
            {
                db db = new db();

                using (SqlDataReader reader = db.Select("SELECT Name FROM Product"))
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    comboBox1.DisplayMember = "Name";
                    comboBox1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Employee data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sale_Load(object sender, EventArgs e)
        {
            LoadProductID();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(comboBox1.Text) ||
                string.IsNullOrWhiteSpace(uprice.Text) ||
                string.IsNullOrWhiteSpace(qty.Text))
            {
                MessageBox.Show("Please fill all the fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate and convert input fields
            if (!decimal.TryParse(uprice.Text, out decimal unitPrice))
            {
                MessageBox.Show("Invalid unit price. Please enter a valid number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(qty.Text, out int quantity))
            {
                MessageBox.Show("Invalid quantity. Please enter a valid whole number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Calculate total for the current row
                decimal rowTotal = unitPrice * quantity;

                // Add a new row to the DataGridView
                int newRow = dataGridView1.Rows.Add();
                dataGridView1.Rows[newRow].Cells[0].Value = comboBox1.Text; // Product Name
                dataGridView1.Rows[newRow].Cells[1].Value = unitPrice.ToString("F2"); // Unit Price (formatted as currency)
                dataGridView1.Rows[newRow].Cells[2].Value = quantity; // Quantity
                dataGridView1.Rows[newRow].Cells[3].Value = rowTotal.ToString("F2"); // Row Total (formatted as currency)

                // Recalculate the overall total
                decimal total = 0;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[3].Value != null &&
                        decimal.TryParse(row.Cells[3].Value.ToString(), out decimal cellTotal))
                    {
                        total += cellTotal;
                    }
                }

                ClearFields();
                // Update the total label
                label5.Text = total.ToString("F2");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comboBox1.Text))
                {
                    MessageBox.Show("Please select a valid product.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                db db = new db();
                using (SqlDataReader reader = db.Select($"SELECT Price FROM Product WHERE Name = '{comboBox1.Text}'"))
                {
                    if (reader != null && reader.Read())
                    {
                        uprice.Text = reader["Price"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("No matching product found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Search_TextChanged(object sender, EventArgs e)
        {
            if (Search.Text != "")
            {
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    if (dataGridView1.Rows[i].Cells[0].Value.ToString().ToLower().Contains(Search.Text.ToLower()))
                    {
                        dataGridView1.Rows[i].Selected = true;
                        comboBox1.Text = dataGridView1.Rows[i].Cells[0].Value.ToString();
                        uprice.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
                        qty.Text = dataGridView1.Rows[i].Cells[2].Value.ToString();
                        label5.Text = dataGridView1.Rows[i].Cells[3].Value.ToString();
                    }
                }
            }

        }
        private void ClearFields()
        {
            comboBox1.Text = "";
            uprice.Text = "";
            qty.Text = "";
            label5.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //when click this need edit the row and update the total
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Validate and convert input fields
                if (!decimal.TryParse(uprice.Text, out decimal unitPrice))
                {
                    MessageBox.Show("Invalid unit price. Please enter a valid number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(qty.Text, out int quantity))
                {
                    MessageBox.Show("Invalid quantity. Please enter a valid whole number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    // Calculate total for the current row
                    decimal rowTotal = unitPrice * quantity;

                    // Update the selected row
                    dataGridView1.SelectedRows[0].Cells[0].Value = comboBox1.Text; // Product Name
                    dataGridView1.SelectedRows[0].Cells[1].Value = unitPrice.ToString("F2"); // Unit Price (formatted as currency)
                    dataGridView1.SelectedRows[0].Cells[2].Value = quantity; // Quantity
                    dataGridView1.SelectedRows[0].Cells[3].Value = rowTotal.ToString("F2"); // Row Total (formatted as currency)

                    // Recalculate the overall total
                    decimal total = 0;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells[3].Value != null &&
                            decimal.TryParse(row.Cells[3].Value.ToString(), out decimal cellTotal))
                        {
                            total += cellTotal;
                        }
                    }
                    ClearFields();
                    // Update the total label
                    label5.Text = total.ToString("F2");
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while updating data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to edit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);

                // Recalculate the overall total
                decimal total = 0;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[3].Value != null &&
                        decimal.TryParse(row.Cells[3].Value.ToString(), out decimal cellTotal))
                    {
                        total += cellTotal;
                    }
                }
                ClearFields();
                // Update the total label
                label5.Text = total.ToString("F2");
               
            }
            else
            {
                MessageBox.Show("Please select a row to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(label5.Text))
            {
                MessageBox.Show("Please add items to the cart before proceeding to payment.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                decimal total = decimal.Parse(label5.Text);
                using (var Payment = new Payment(total))
                {
                    Payment.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while processing payment: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
