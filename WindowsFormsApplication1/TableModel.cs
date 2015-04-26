using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    interface TableModel
    {
        DataTable getData();
        void changeTable(string tableName);
        void update();
        DataColumn[] getPrimaryKeys();
        DataColumnCollection getColumns();
    }

    class DefaultTableModel : TableModel
    {
        private SqlConnection connection;
        private string tableName;

        private SqlCommand cmd = null;
        private DataSet data = new DataSet();
        private SqlDataAdapter dataAdapter;
        private DataTable table;
        
        
        public DefaultTableModel(SqlConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException();
            this.connection = connection;
        }

        public DefaultTableModel(SqlConnection connection, string tableName)
        {
            if (connection == null)
                throw new ArgumentNullException();
            this.connection = connection;
            this.tableName = tableName;
            fetchTableData();
        }

        public DataTable getData()
        {
            return table;
        }

        public void changeTable(string tableName)
        {
            
            if(tableName==null)
                throw new ArgumentNullException("Cannot set model table name to null.");
            if(tableName.Length<1)
                throw new ArgumentException("Unproper table name.");
            this.tableName = tableName;
            Console.WriteLine(tableName);
            fetchTableData();
        }

        public void update()
        {
            if(tableName.Length>1)
                fetchTableData();
        }

        public DataColumn[] getPrimaryKeys()
        {
            return table.PrimaryKey;
        }

        public DataColumnCollection getColumns()
        {
            return table.Columns;
        }

        private void fetchTableData()
        {
            string command = "SELECT * FROM " + tableName;
            cmd = new SqlCommand(command, connection);

            try
            {
                dataAdapter = new SqlDataAdapter(command, connection);
                table = new DataTable();
                if(table!=null)
                dataAdapter.Fill(table);
            }
            catch (SqlException e)
            {
                Console.WriteLine("SQLException!");
                Console.WriteLine(e.StackTrace.ToString());
            }

        }
    }
}
