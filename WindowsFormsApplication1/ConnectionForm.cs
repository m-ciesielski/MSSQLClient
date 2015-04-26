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

namespace WindowsFormsApplication1
{
    public partial class ConnectionForm : Form
    {
        public ConnectionForm()
        {
            InitializeComponent();
        }



        private void openConnection(SqlConnection conn)
        {
            try
            {
                conn.Open();

            }
            catch (SqlException e)
            {
                Console.WriteLine("{0}", e.StackTrace.ToString());
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            //  bool valid = true;
            if (userTextBox.Text.Length == 0 || serverTextBox.Text.Length == 0 ||
                catalogTextBox.Text.Length == 0)
            {
                // valid = false;
                Console.WriteLine("Invalid");
                if (userTextBox.Text.Length == 0)
                    userTextBox.BackColor = Color.Red;
                if (serverTextBox.Text.Length == 0)
                    userTextBox.BackColor = Color.Red;
                if (catalogTextBox.Text.Length == 0)
                    userTextBox.BackColor = Color.Red;
            }
            else
            {
                Console.WriteLine("Connecting...");
                string connString = "Data Source=" + serverTextBox.Text + "; Initial Catalog=" + catalogTextBox.Text + ";User=" + userTextBox.Text + ";Password=" + passwordTextBox.Text;
                SqlConnection connection = new SqlConnection(connString);
                openConnection(connection);
                MainForm mainForm = new MainForm(connection);
                this.Hide();
                mainForm.Show();
            }
             
        }
    }
}
