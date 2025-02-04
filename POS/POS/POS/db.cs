using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    internal class db
    {
        string ConnectionString = "Data Source=OGOTECHNOLOGY;Initial Catalog=SuperMarketDBPOS;Integrated Security=True";

        SqlConnection conn = null;

        public db()
        {
            conn = new SqlConnection(ConnectionString);
        }


        public void Execute(string Query)
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(Query, conn);
                cmd.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            finally
            {
                conn.Close();
            }

        }
        public SqlDataReader Select(string Query)
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(Query, conn);
                return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }


        public int ExecuteScalar(string query, SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddRange(parameters);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}
