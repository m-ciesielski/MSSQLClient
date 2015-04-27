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
        List<string[]> getForeignKeys();
        DataColumnCollection getColumns();
        string getTableName();
    }

    class DefaultTableModel : TableModel
    {
        private SqlConnection connection;
        private string tableName;

        private SqlCommand cmd = null;
        private DataSet data = new DataSet();
        private SqlDataAdapter dataAdapter;
        private DataTable table;
        private DataTable schema;
        private List<string[]> foreignKeys;
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
            fetchForeignKeys();
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
            fetchForeignKeys();
        }

        public void update()
        {
            if(tableName.Length>1)
                fetchTableData();
        }

        public DataColumn[] getPrimaryKeys()
        {
            return schema.PrimaryKey;
        }

        public List<string[]> getForeignKeys()
        {
            return foreignKeys;
        }


        /**
         * Pobiera listę kluczy obcych. Pierwszy  element klucza to nazwa tablicy do której odnosi się klucz obcy, 
         * Drugi element to indeks kolumny klucza obcego.
         * */
        private void fetchForeignKeys()
        {


            string command = "SELECT object_name(f.parent_object_id) ParentTableName," +
                             " object_name(f.referenced_object_id) RefTableName, " +
                             "c.parent_column_id " +
                             "FROM sys.foreign_keys f JOIN sys.foreign_key_columns c " +
                             "on f.object_id=c.constraint_object_id " +
                             "WHERE f.parent_object_id = object_id('" + tableName + "');";
            cmd = new SqlCommand(command, connection);

            foreignKeys = new List<string[]>();
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string[] foreignKey={reader.GetString(1), reader.GetInt32(2).ToString()};
                    foreignKeys.Add(foreignKey);
                }
                    

                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e.StackTrace.ToString());
            }
        }

        public DataColumnCollection getColumns()
        {
            return table.Columns;
        }

        public string getTableName()
        {
            return tableName;
        }

        private void fetchTableData()
        {
            string command = "SELECT * FROM " + tableName;
            cmd = new SqlCommand(command, connection);

            try
            {
               using (dataAdapter = new SqlDataAdapter(command, connection))
                {
                    table = new DataTable();
                    if(table!=null)
                    dataAdapter.Fill(table);
                    schema = new DataTable();
                    dataAdapter.FillSchema(schema, SchemaType.Mapped);
                }
                
                

            }
            catch (SqlException e)
            {
                Console.WriteLine("SQLException!");
                Console.WriteLine(e.StackTrace.ToString());
            }

        }
    }
}
