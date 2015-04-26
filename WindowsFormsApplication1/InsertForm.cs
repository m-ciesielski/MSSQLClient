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

        public InsertForm(DataColumnCollection columns, DataColumn[] primaryKeys)
        {
            this.columns = columns;
            this.primaryKeys = primaryKeys;
            initialize();
            InitializeComponent();
        }

        private void initialize()
        {
            panel1 = new Panel();
            panel1.BackColor = Color.Azure;
            panel1.Location = new System.Drawing.Point(50, 50);
            panel1.Anchor = AnchorStyles.Top;
            panel1.Size = new Size(200, 60 * columns.Count);

            insertButton = new Button();
            insertButton.Location = new System.Drawing.Point(50, panel1.Size.Height+50);
            insertButton.Anchor = AnchorStyles.Left;

            this.Controls.Add(panel1);
            this.Controls.Add(insertButton);
            for(int i=0;i<columns.Count;++i)
            {
                TextBox textBox = new TextBox();
                textBox.Location = new System.Drawing.Point(50, 10 + 50 * i);
                panel1.Controls.Add(textBox);
            }
             
        }
    }
}
