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
        private DefaultMainController viewModel;
        private SqlConnection connection;
        private int lastIndex = -1;
        //private DataGridViewButtonColumn deleteColumn;
        private DataGridViewButtonColumn updateColumn;
        private List<DataTable> tables;
        private List<DataTable> schemas;
        public MainForm(SqlConnection connection)
        {
            if(connection==null)
                throw new ArgumentNullException();
            this.connection=connection;

            viewModel = new DefaultMainController(this ,connection);
            InitializeComponent();

            if(Program.connectedMode)
                toolStripStatusLabel1.Text = "Tryb połączeniowy";
            else
                toolStripStatusLabel1.Text = "Tryb bezpołączeniowy";

            updateTableNames(viewModel.getTableNames());

            if (!Program.connectedMode)
                loadTables();

            /*
            deleteColumn = new DataGridViewButtonColumn();
            deleteColumn.HeaderText = "Usuwanie";
            deleteColumn.Text = "Usuń";
            deleteColumn.Name = "deleteColumn";
            deleteColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(deleteColumn);
            */

            updateColumn = new DataGridViewButtonColumn();
            updateColumn.HeaderText = "Edycja";
            updateColumn.Text = "Edytuj";
            updateColumn.Name = "updateColumn";
            updateColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(updateColumn);
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            
        }

        public void update(DataTable table)
        {
            dataGridView1.DataSource = table;
            dataGridView1.Update();
            
        }

        //TODO: move to DatabaseOverviewModel?
        private void loadTables()
        {
            tables = new List<DataTable>();
            schemas = new List<DataTable>();
            List<string> tableNames= viewModel.getTableNames();
            for (int i = 0; i < tableNames.Count; ++i)
            {
                TableModel model = new DefaultTableModel(connection, tableNames[i]);
                tables.Add(model.getData());
                schemas.Add(model.getSchema());
                tables[i].TableName = tableNames[i];
            }
        }

        public List<DataTable> getTables()
        {
            return tables;
        }


        public List<DataTable> getSchemas()
        {
            return schemas;
        }

        //TODO:clear list?
        public void updateTableNames(List<string> tableNames)
        {
            listBox1.Items.Clear();
            for (int i = 0; i < tableNames.Count; ++i)
            {
                listBox1.Items.Add(tableNames[i]);
            }
        }

        public int getSelectedIndex()
        {

            if (dataGridView1.SelectedRows.Count > 0)
                return dataGridView1.SelectedRows[0].Index;
            else if (dataGridView1.SelectedCells.Count > 0)
                return dataGridView1.SelectedCells[0].RowIndex;
            else
                return -1;
        }

        public List<int> getSelectedIndices()
        {
            List<int> indices = new List<int>();

            for (int i = 0; i < dataGridView1.SelectedRows.Count; ++i)
                if (dataGridView1.SelectedRows[i].Index < dataGridView1.RowCount - 1)
                    indices.Add(dataGridView1.SelectedRows[i].Index);

            if(indices.Count==0)
                for (int i = 0; i < dataGridView1.SelectedCells.Count; ++i)
                    if(dataGridView1.SelectedCells[i].RowIndex<dataGridView1.RowCount-1)
                        indices.Add(dataGridView1.SelectedCells[i].RowIndex);

                return indices;
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

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex!=-1)
            {
                DialogResult deleteDialog = MessageBox.Show(createDeleteMessage(),
                   "Potwierdzenie usunięcia", MessageBoxButtons.OKCancel);
                if(deleteDialog==DialogResult.OK)
                {
                    viewModel.delete(getSelectedIndices());
                }

            }

            
        }

        private string createDeleteMessage()
        {
            StringBuilder buff= new StringBuilder();
            List<int> indices= getSelectedIndices();

            buff.Append("Czy chcesz usunąć rekordy o pozycjach: ");
            for (int i = 0; i < indices.Count; ++i)
                buff.Append((indices[i]+1)+", ");

            buff.Append("?");

            return buff.ToString();
        }

        private void trybToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void połaczeniowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.connectedMode = true;
            toolStripStatusLabel1.Text = "Tryb połączeniowy";
        }

        private void bezpołaczeniowyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.connectedMode = false;
            toolStripStatusLabel1.Text = "Tryb bezpołączeniowy";
            if (tables == null || schemas == null)
                loadTables();

        }

        private void syncButton_Click(object sender, EventArgs e)
        {
            viewModel.sync();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                UpdateForm updateForm = new UpdateForm(viewModel, e.RowIndex);
                updateForm.Show();
            }

            else if(senderGrid.Columns[e.ColumnIndex].CellType==typeof(DataGridViewImageCell))
            {
                ImageForm imageForm = new ImageForm(viewModel, senderGrid.Columns[e.ColumnIndex].Name, e.RowIndex);
                imageForm.Show();
            }
        }


    }
}
