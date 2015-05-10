using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
namespace WindowsFormsApplication1
{
    public partial class UpdateForm : Form
    {

        private DataColumnCollection columns;
        private DataColumn[] primaryKeys;
        private DataColumn identityColumn;
        private List<string[]> foreignKeys;
        private MainController controller;

        private List<Control> inputFields;
        private List<List<int>> foreignKeysIds;

        private int index;


        public UpdateForm(MainController controller, int index)
        {
            if (controller == null)
                throw new ArgumentNullException();
            this.controller = controller;
            this.columns = controller.getColumns();
            this.primaryKeys = controller.getPrimaryKeys();
            this.foreignKeys = controller.getForeignKeys();
            this.identityColumn = controller.getIdentityColumn();
            this.index = index;
            InitializeComponent();
            initialize();
        }


        protected List<object> getInsertedValues()
        {
            List<Object> values = new List<object>();
           // int j = 0;
           // foreign key counter
            int k = 0;
            for (int i = 0, j=0; i < inputFields.Count; ++i, ++j)
            {
                if (Program.connectedMode && columns[i].ColumnName == identityColumn.ColumnName)
                    ++j;

                    
                if(inputFields[i].GetType()==typeof(TextBox))
                {
                    if(columns[j].DataType == typeof(string))
                    values.Add(inputFields[i].Text);
                    else if (columns[j].DataType == typeof(int))
                        values.Add(Convert.ToInt32(inputFields[i].Text));
                    else if (columns[j].DataType == typeof(double))
                        values.Add(Convert.ToDouble(inputFields[i].Text));
                    else if (columns[j].DataType == typeof(bool))
                        values.Add(Convert.ToBoolean(inputFields[i].Text));
                    else if (columns[j].DataType == typeof(DateTime))
                        values.Add(DateTime.ParseExact(inputFields[i].Text, "dd-MM-yyyy", CultureInfo.InvariantCulture));
                }
                else if (inputFields[i].GetType() == typeof(ComboBox))
                {
                    ComboBox combo = (ComboBox)inputFields[i];
                    values.Add(foreignKeysIds[k][combo.SelectedIndex]);
                    ++k;
                }
            }
                
            
            for (int i = 0; i < values.Count; ++i)
                Console.WriteLine(values[i]);
            
            return values;
        }

        protected void initialize()
        {

            inputFields = new List<Control>();

            panel1.BackColor = Color.Azure;
            panel1.Location = new System.Drawing.Point(50, 50);
            panel1.Dock = DockStyle.Top;
            panel1.Size = new Size(400, 50 * columns.Count);

 
            updateButton.Location = new System.Drawing.Point(50, panel1.Size.Height+50);
          //  updateButton.Anchor = AnchorStyles.Right;
            updateButton.Text = "Modyfikuj";

            List<DataTable> refTables=null;
            bool comboAdded = false;

            if(foreignKeys.Count>0)
            {
                refTables = controller.getReferencedTablesData();
                foreignKeysIds = new List<List<int>>();
            }


            for(int i=0;i<columns.Count;++i)
            {
                comboAdded = false;
                //Fkeys
                for(int j=0;j<foreignKeys.Count;++j)
                {
                    if((i+1)==Convert.ToInt32(foreignKeys[j][1]))
                    {
                        Label comboLabel = new Label();
                        comboLabel.Text = columns[i].ColumnName;
                        comboLabel.Location = new System.Drawing.Point(0, 10 + 50 * i);

                        ComboBox combo = new ComboBox();
                        foreignKeysIds.Add(new List<int>());
                        for (int k = 0; k < refTables[j].Rows.Count; ++k)
                        {
                            foreignKeysIds.Last().Add((int)refTables[j].Rows[k].ItemArray[0]);
                            StringBuilder strBuilder = new StringBuilder();
                            for (int l = 1; l < refTables[j].Rows[k].ItemArray.Length; ++l)
                                strBuilder.Append(Convert.ToString(refTables[j].Rows[k].ItemArray[l])+" ");

                            combo.Items.Add(strBuilder.ToString());
                        }
      
                            combo.Location = new System.Drawing.Point(comboLabel.Width + 25, 10 + 50 * i);

                            combo.SelectedIndex = (int)controller.getValues(index)[i]-1;
                            
                            panel1.Controls.Add(combo);
                            inputFields.Add(combo);
                            panel1.Controls.Add(comboLabel);
                            comboAdded = true;
                            break;
                    }
                }
                if (comboAdded)
                    continue;

                if (Program.connectedMode == true && identityColumn!=null &&
                    columns[i].ColumnName == identityColumn.ColumnName)
                    continue;
                    


                Label label = new Label();
                label.Text = columns[i].ColumnName;
                label.Location = new System.Drawing.Point(0, 10 + 50 * i);

                TextBox textBox = new TextBox();
                if(columns[i].DataType == typeof(int))
                    textBox.Validating+=validateIntegerTextBox;

                else if (columns[i].DataType == typeof(double) ||
                    columns[i].DataType == typeof(float))
                    textBox.Validating += validateDecimalTextBox;

                else if (columns[i].DataType == typeof(bool))
                    textBox.Validating += validateBooleanTextBox;

                else if (columns[i].DataType == typeof(DateTime))
                    textBox.Validating += validateDateTextBox;

                for (int j = 0; j < primaryKeys.Length;++j )
                {
                    if (primaryKeys[j].ColumnName==columns[i].ColumnName)
                        textBox.BackColor = Color.Azure;
                }
                
                
 

                textBox.Location = new System.Drawing.Point(label.Width + 25, 10 + 50 * i);

                textBox.Text = controller.getValues(index)[i].ToString();

                panel1.Controls.Add(label);
                panel1.Controls.Add(textBox);
                inputFields.Add(textBox);
            }

            

             
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
                controller.update(index, getInsertedValues());
                if (Program.connectedMode)
                    controller.updateAll();
                else
                    controller.updateView();

                this.Hide();
        }

        protected void validateDecimalTextBox(object sender, CancelEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            for (int i = 0; i < textBox.Text.Length; ++i)
                if ((textBox.Text[i] < '0' || textBox.Text[i] > '9') &&
                    (textBox.Text[i] != ' ' && textBox.Text[i] != ',' && textBox.Text[i] != '.'))
                    e.Cancel = true;

            if(e.Cancel==true)
                    MessageBox.Show("Niepoprawna wartość: "+ textBox.Text);
                    
        }

        protected void validateIntegerTextBox(object sender, CancelEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            for (int i = 0; i < textBox.Text.Length; ++i)
                if (textBox.Text[i] < '0' || textBox.Text[i] > '9')
                    e.Cancel = true;

            if (e.Cancel == true)
                MessageBox.Show("Niepoprawna wartość: " + textBox.Text);
                    
        }

        protected void validateBooleanTextBox(object sender, CancelEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text != "true" && textBox.Text != "false" &&
                textBox.Text != "True" && textBox.Text != "False" &&
                textBox.Text!="1" && textBox.Text!="0")
                e.Cancel = true;

            if (e.Cancel == true)
                MessageBox.Show("Niepoprawna wartość: " + textBox.Text);
        }

        protected void validateDateTextBox(object sender, CancelEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (DateTime.ParseExact(textBox.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture) == null)
                e.Cancel = true;

            if (e.Cancel == true)
                MessageBox.Show("Niepoprawna wartość: " + textBox.Text);
        }
    
    }
}
