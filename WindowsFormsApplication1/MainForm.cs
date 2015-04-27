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
    public partial class MainForm : Form
    {
        //private SqlConnection conn;
        private DefaultTableViewModel viewModel;
        private SqlConnection connection;
        private int lastIndex = -1;
        private DataGridViewButtonColumn deleteColumn;
        private DataGridViewButtonColumn updateColumn;
        public MainForm(SqlConnection connection)
        {
            if(connection==null)
                throw new ArgumentNullException();
            this.connection=connection;

            viewModel = new DefaultTableViewModel(this ,connection);
            InitializeComponent();
            updateTableNames(viewModel.getTableNames());


            deleteColumn = new DataGridViewButtonColumn();
            deleteColumn.HeaderText = "Usuwanie";
            deleteColumn.Text = "Usuń";
            deleteColumn.Name = "deleteColumn";
            deleteColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(deleteColumn);

            updateColumn = new DataGridViewButtonColumn();
            updateColumn.HeaderText = "Edycja";
            updateColumn.Text = "Edytuj";
            updateColumn.Name = "updateColumn";
            updateColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(updateColumn);
            
        }

        public void update(DataTable table)
        {
            dataGridView1.DataSource = table;
            
        }

        //TODO:clear list?
        public void updateTableNames(List<string> tableNames)
        {
            for (int i = 0; i < tableNames.Count; ++i)
            {
                listBox1.Items.Add(tableNames[i]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            viewModel.changeTable(listBox1.GetItemText(listBox1.SelectedItem));
        }



        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            Console.WriteLine("Closing...");
            connection.Close();
            Application.Exit();
        }

 

        private void MainForm_Load(object sender, EventArgs e)
        {
        
        }

        private void MainForm_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("Closing..");
            Application.Exit();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lastIndex!=listBox1.SelectedIndex)
            {
                lastIndex = listBox1.SelectedIndex;
                viewModel.changeTable(listBox1.GetItemText(listBox1.SelectedItem));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex!=-1)
            {
                InsertForm insertForm = new InsertForm(viewModel);
                insertForm.Show();
            }
            
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}
