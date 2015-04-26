using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Data;
using System.Data.SqlClient;

/**
 * TODO: Logs
 * TODO: Insert
 * TODO: Update
 * TODO: Delete
 * TODO: Primary keys handling
 * TODO: Composite PK handling
 * TODO: modes switch
 * 
 * */

namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SqlConnection connection = new SqlConnection("Data Source=eos.inf.ug.edu.pl; Initial Catalog=mciesielski;User=mciesielski;Password=224626");
            

            try
            {
                connection.Open();
                
                //Form1.setData(data);
               
                
   
            }
            catch (SqlException e)
            {
                Console.WriteLine("{0}",e.StackTrace.ToString());
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ConnectionForm connectionForm = new ConnectionForm();

           // MainForm mainForm = new MainForm();
           // mainForm.connection = connection;
          //  mainForm.setData(data);
            Application.Run(connectionForm);


        }
    }
}
