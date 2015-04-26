using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    interface DatabaseOverviewModel
    {
        void update();
        List<string> getTableNames();
    }
    class DefaultDatabaseOverviewModel : DatabaseOverviewModel
    {
        private SqlConnection connection;
        private List<string> tableNames;

        public DefaultDatabaseOverviewModel(SqlConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException();
            this.connection = connection;
            fetchTableNames();
        }

        public List<string> getTableNames()
        {
            return tableNames;
        }

        public void update()
        {
            fetchTableNames();
        }

        private void fetchTableNames()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES", connection);
            tableNames = new List<string>();
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    tableNames.Add(reader.GetString(2));

                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e.StackTrace.ToString());
            }

            

        }
        
    }
}
