using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class ImageForm : Form
    {

        private MainController viewModel;
        private string columnName;
        private int index;
        public ImageForm(MainController viewModel, string columnName, int index)
        {
            this.viewModel = viewModel;
            this.columnName = columnName;
            this.index = index;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(imagePathTextBox.Text.Length>0)
            {
                viewModel.insertImage(imagePathTextBox.Text, columnName, index);
                viewModel.updateAll();
                this.Hide();
            }
        }
    }
}
