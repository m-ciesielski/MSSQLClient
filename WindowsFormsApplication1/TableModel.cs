using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    interface TableModel
    {
        DataTable getData();
        DataTable getSchema();
        void changeTable(string tableName);
        void changeTableDisconnected(string tableName, DataTable table, DataTable schema);
        void update();
        void updateDisconnected(int index, List<Object> values);
        void updateConnected(int index, List<Object> values);
        void execute(string command);
        void deleteDisconnected(List<int> indices);
        void deleteConnected(List<int> indices);
        void insert(List<Object> values);
        void insertImage(string path, string columnName, int index);
        void sync();
        DataColumn[] getPrimaryKeys();
        List<string[]> getForeignKeys();
        List<Object> getValues(int index);
        DataColumn getIdentityColumn();
        DataColumnCollection getColumns();
        string getTableName();
        SqlConnection getConnection();
    }

    class DefaultTableModel : TableModel
    {
        protected SqlConnection connection;
        protected string tableName;

        protected SqlCommand cmd = null;
       // protected DataSet data = new DataSet();
        protected SqlDataAdapter dataAdapter;
        private DataTable table;
        private DataTable schema;
        protected List<string[]> foreignKeys;
        private SqlCommandBuilder commandBuilder;

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
            fetchTableData(tableName);
            fetchForeignKeys();
        }

        public DataTable getData()
        {
            return table;
        }

        public void sync()
        {
            dataAdapter.InsertCommand=commandBuilder.GetInsertCommand();
            dataAdapter.DeleteCommand=commandBuilder.GetDeleteCommand();
            dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
            try
            {
                dataAdapter.Update(table);
                MessageBox.Show("Synchronizacja powiodła się.\n");
            }
            catch (SqlException e)
            {
                MessageBox.Show("Synchronizacja nie powiodła się.\n"+e.Errors.ToString());
               
            }
            
        }

        public DataTable getSchema()
        {
            return schema;
        }

        public SqlConnection getConnection()
        {
            return connection;
        }

        public void changeTable(string tableName)
        {
            
            if(tableName==null)
                throw new ArgumentNullException("Cannot set model table name to null.");
            if(tableName.Length<1)
                throw new ArgumentException("Unproper table name.");
            this.tableName = tableName;
            Console.WriteLine(tableName);
            fetchTableData(tableName);
            commandBuilder = new SqlCommandBuilder(dataAdapter);
            fetchForeignKeys();
        }

        public void changeTableDisconnected(string tableName, DataTable table, DataTable schema)
        {

            if (tableName == null)
                throw new ArgumentNullException("Cannot set model table name to null.");
            if (tableName.Length < 1)
                throw new ArgumentException("Unproper table name.");
            this.tableName = tableName;
            this.table = table;
            this.schema = schema;
            string command = "SELECT * FROM " + tableName;
            dataAdapter = new SqlDataAdapter(command, connection);
            fetchForeignKeys();
            commandBuilder = new SqlCommandBuilder(dataAdapter);
        }

        public void update()
        {
                fetchTableData(tableName);
        }

        public void execute(string command)
        {
            try
            {
                using(cmd = new SqlCommand(command, connection))
                cmd.ExecuteNonQuery();
            }
            catch(SqlException e)
            {
                Console.WriteLine("SqlException encountered while executing command: "+ command);
                MessageBox.Show("Nieprawidłowe polecenie: "+command);
            }
            
            
        }

        public DataColumn getIdentityColumn()
        {
            for (int i = 0; i < schema.PrimaryKey.Length; ++i)
                if (schema.PrimaryKey[i].AutoIncrement)
                    return schema.PrimaryKey[i];

            return null;
        }

        public void insert(List<Object> values)
        {
            table.Rows.Add(values.ToArray());
        }

        //TODO: connected/disconnected
        public void insertImage(string path, string columnName, int index)
        {
            byte[] imageData;
            try
            {
                imageData = getByteArray(path);
            }
            catch (IOException e)
            {
                //TODO:message box
                Console.WriteLine("Nie znaleziono pliku pod adresem: " + path);
                
                return;
            }
            
            string str="UPDATE "+tableName+" SET "+columnName+"= @ImageData "+
                "WHERE id_Pracownik = "+getPrimaryKeysValues(index)[0].ToString();
            SqlCommand command = new SqlCommand(str, connection);
            command.Parameters.Add(new SqlParameter("@ImageData", (object)imageData));

            List<Object> values =getValues(index);

           

           // Command comm=new Command(Command.MainStatement.INSERT, tableName, getColumns(), )
            command.ExecuteNonQuery();
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
        protected void fetchForeignKeys()
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

        public void deleteConnected(List<int> indices)
        {
            

            if (indices == null || indices.Count == 0)
                return;

            if (table == null)
                throw new InvalidOperationException();

            List<int> pkValues = new List<int>();

            for (int i = 0; i < indices.Count; ++i)
            {

                pkValues = getPrimaryKeysValues(indices[i]);

                Command deleteCommand = new Command(Command.MainStatement.DELETE,
                    tableName, Command.PrimaryKeyColumnsToStringList(getPrimaryKeys()),
                    pkValues.ToArray());

                Console.WriteLine(deleteCommand.getCommandString());

                execute(deleteCommand.getCommandString());

                pkValues.Clear();
            }

        }

        public List<int> getPrimaryKeysValues(int index)
        {
            List<int> pkValues = new List<int>();
            DataColumn[] pkColumns = getPrimaryKeys();
            for (int i = 0; i < table.Columns.Count; ++i)
            {
                for (int j = 0; j < getPrimaryKeys().Length; ++j)
                    if (table.Columns[i].ColumnName == pkColumns[j].ColumnName)
                        pkValues.Add((int)table.Rows[index].ItemArray[j]);
            }

            return pkValues;
        }

        public List<Object> getValues(int index)
        {
            List<Object> values = new List<Object>();
            for (int i = 0; i < table.Rows[index].ItemArray.Length; ++i)
            {
                        values.Add(table.Rows[index].ItemArray[i]);
            }

            return values;
        }

        public void updateDisconnected(int index, List<Object> values)
        {
            if (index > table.Rows.Count)
                return;

            for (int i = 0; i < table.Rows[index].ItemArray.Length; ++i)
                    if (!table.Rows[index].ItemArray[i].Equals(values[i]))
                    {
                        table.Rows[index].BeginEdit();
                        table.Rows[index][i] = values[i];
                        table.Rows[index].EndEdit();
                    }
                        
                    
        }

        public void updateConnected(int index, List<Object> values)
        {
            if (index > table.Rows.Count)
                return;

           // List<int> pkValues = getPrimaryKeysValues(index);


            int j = 0;

            for (int i = 0; i < table.Rows[index].ItemArray.Length; ++i)
            {
                if (table.Columns[i].ColumnName==getIdentityColumn().ColumnName)
                {
                    Console.WriteLine(table.Columns[i].ColumnName);
                }
                    

                else 
                {
                    if (!table.Rows[index].ItemArray[i].Equals(values[j]))
                    {
                        Command updateCommand = new Command(Command.MainStatement.UPDATE,
                        tableName, getPrimaryKeys(), getPrimaryKeysValues(index).ToArray(),
                        values[j], table.Columns[i].ColumnName);

                        Console.WriteLine(updateCommand.getCommandString());

                        execute(updateCommand.getCommandString());
                    }
                    
                    ++j;
                }
            }
                    
            
        }

        public void deleteDisconnected(List<int> indices)
        {

            if (indices == null || indices.Count == 0)
                return;

            if (table == null)
                throw new InvalidOperationException();

            for (int i = 0; i < indices.Count; ++i)
                table.Rows[indices[i]].Delete();
        
        }


        protected byte[] getByteArray(string sPath)
        {
            byte[] data = null;

            FileInfo fInfo = new FileInfo(sPath);

            long numBytes = fInfo.Length;

            FileStream fStream = new FileStream(sPath, FileMode.Open,
            FileAccess.Read);

            BinaryReader br = new BinaryReader(fStream);
            data = br.ReadBytes((int)numBytes);

            return data;

        }

        protected void fetchTableData(string tableName)
        {
            string command = "SELECT * FROM " + tableName;
            cmd = new SqlCommand(command, connection);

            try
            {
                    dataAdapter = new SqlDataAdapter(command, connection);
                    table = new DataTable();
                    if(table!=null)
                     dataAdapter.Fill(table);
                    schema = new DataTable();
                    dataAdapter.FillSchema(schema, SchemaType.Mapped);
                    
               

            }
            catch (SqlException e)
            {
                Console.WriteLine("SQLException on fetching data from: "+ tableName);
                Console.WriteLine(e.StackTrace.ToString());
            }

        }

        protected void fetchTableData(string tableName, DataTable table, DataTable schema)
        {
            string command = "SELECT * FROM " + tableName;

            try
            {
                    dataAdapter = new SqlDataAdapter(command, connection);
                    if (table != null)
                        dataAdapter.Fill(table);
                    dataAdapter.FillSchema(schema, SchemaType.Mapped);



            }
            catch (SqlException e)
            {
                Console.WriteLine("SQLException!");
                Console.WriteLine(e.StackTrace.ToString());
            }

        }

        protected List<string> GetReferencedTableNames()
        {
            List<string> referencedTableNames= new List<string>();

            for (int i = 0; i < foreignKeys.Count; ++i)
                referencedTableNames.Add(foreignKeys[i][0]);
            return referencedTableNames;
        }
    }

    interface ExtendedTableModel : TableModel
    {
        List<DataTable> getReferencedTablesData();
    }


    class ExtendedDefaultTableModel : DefaultTableModel, ExtendedTableModel
    {
        protected List<string> referencedTableNames;
        protected List<DataTable> referencedTables;
        protected List<DataTable> referencedSchemas;

        public ExtendedDefaultTableModel(SqlConnection connection) : base(connection)
        {
            this.connection = connection;
            
        }


        public ExtendedDefaultTableModel(SqlConnection connection,string tableName, List<string> referencedTableNames) : base(connection, tableName)
        {
            this.connection = connection;
            this.referencedTableNames = referencedTableNames;
            this.tableName = tableName;
            fetchReferencedTablesData();
        }
        

        protected void fetchReferencedTablesData()
        {
            if (foreignKeys.Count == 0)
                return;
            referencedTables = new List<DataTable>();
            referencedSchemas = new List<DataTable>();

            for(int i=0; i< foreignKeys.Count; ++i)
            {
                referencedTables.Add(new DataTable());
                referencedSchemas.Add(new DataTable());
                fetchTableData(foreignKeys[i][0], referencedTables[i], referencedSchemas[i]);
            }
        }

        public List<DataTable> getReferencedTablesData()
        {
            return referencedTables;
        }

        

        public void changeTable(string tableName)
        {
            base.changeTable(tableName);
            this.referencedTableNames = base.GetReferencedTableNames();
            fetchReferencedTablesData();
        }



    }
}
