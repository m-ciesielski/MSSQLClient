using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace WindowsFormsApplication1
{
    public partial class InsertForm : Form
    {
        private DataColumnCollection columns;
        private DataColumn[] primaryKeys;
        private List<string[]> foreignKeys;
        private TableViewModel viewModel;

        public InsertForm(TableViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException();
            this.viewModel = viewModel;
            this.columns = viewModel.getColumns();
            this.primaryKeys = viewModel.getPrimaryKeys();
            this.foreignKeys = viewModel.getForeignKeys();
            InitializeComponent();
            initialize();
        }

        private void initialize()
        {
           // panel1 = new Panel();
            panel1.BackColor = Color.Azure;
            panel1.Location = new System.Drawing.Point(50, 50);
            panel1.Dock = DockStyle.Top;
            panel1.Size = new Size(400, 60 * columns.Count);

           // insertButton = new Button();
            insertButton.Location = new System.Drawing.Point(50, panel1.Size.Height+50);
            insertButton.Anchor = AnchorStyles.Right;
            insertButton.Text = "Dodaj rekord";
           // this.Controls.Add(panel1);
           // this.Controls.Add(insertButton);
            for(int i=0;i<columns.Count;++i)
            {
                Label label = new Label();
                label.Text = columns[i].ColumnName;
                label.Location = new System.Drawing.Point(0, 10 + 50 * i);

                TextBox textBox = new TextBox();
                for (int j = 0; j < primaryKeys.Length;++j )
                {
                    if (primaryKeys[j].ColumnName==columns[i].ColumnName)
                        textBox.BackColor = Color.Azure;
                }

                for (int j = 0; j < foreignKeys.Count; ++j)
                {
                    if (Convert.ToInt32(foreignKeys[j][1])-1 == i)
                        textBox.BackColor = Color.Bisque;
                }


                textBox.Location = new System.Drawing.Point(label.Width + 25, 10 + 50 * i);

                panel1.Controls.Add(label);
                panel1.Controls.Add(textBox);
            }

            

             
        }
        //TODO: get inserted values
        private void insertButton_Click(object sender, EventArgs e)
        {
            List<Object> testValues= new List<object>();
            testValues.Add(1);
            testValues.Add(2);
            Command insertCommand = new Command(Command.MainStatement.INSERT, viewModel.getTableName(), viewModel.getColumns(), testValues);
            Console.WriteLine(insertCommand.createCommandString());
        }
    }
}
