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
        public static bool connectedMode = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ConnectionForm connectionForm = new ConnectionForm();
            Application.Run(connectionForm);


        }
    }
}
