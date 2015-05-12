using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public interface MainController
    {
        void updateAll();
        void updateView();
        void updateModel();
        void changeTable(string tableName);
        void execute(string command);
        void delete(List<int> indices);
        void insert(List<Object> values);
        void insertImage(string path, string columnName, int index);
        void update(int index, List<Object> values);
        void sync();
        string getTableName();
        List<string> getTableNames();
        List<string[]> getForeignKeys();
        DataColumn[] getPrimaryKeys();
        List<Object> getValues(int index);
        DataColumnCollection getColumns();
        DataColumnCollection getColumnsWithoutIdentity();
        DataColumn getIdentityColumn();
        List<DataTable> getReferencedTablesData();
    }

    class DefaultMainController : MainController
    {
        private TableModel model;
        private MainForm view;
        private DatabaseOverviewModel overviewModel;
        private ExtendedTableModel extendedModel;

        public DefaultMainController(MainForm view, SqlConnection connection)
        {
            if (view == null || connection==null)
                throw new ArgumentNullException();
            this.view = view;
            this.model = new DefaultTableModel(connection);
            this.overviewModel = new DefaultDatabaseOverviewModel(connection);
        }

        //TODO: delete?
        public DefaultMainController(MainForm view, TableModel model, DatabaseOverviewModel overviewModel)
        {
            if (view == null)
                throw new ArgumentNullException();
            this.view = view;
            this.model = model;
            this.overviewModel = overviewModel;
        }

        public void updateModel()
        {
            model.update();
        }

        public void updateView()
        {
            view.update(model.getData());
        }

        public void update(int index, List<Object> values)
        {
            if (Program.connectedMode)
                model.updateConnected(index, values);
            else
                model.updateDisconnected(index, values);

            updateView();
        }

        public void execute(string command)
        {
            model.execute(command);
        }

        public List<Object> getValues(int index)
        {
            return model.getValues(index);
        }

        public void insertImage(string path, string columnName, int index)
        {
            model.insertImage(path, columnName, index);
        }

        public void changeTable(string tableName)
        {
            if (model != null)
            {
                if(Program.connectedMode)
                {
                    model.changeTable(tableName);

                    if (model.getForeignKeys().Count > 0)
                    {
                        this.extendedModel = new ExtendedDefaultTableModel(model.getConnection());
                        extendedModel.changeTable(tableName);
                    }

                    updateAll();
                }
                else
                {
                    DataTable table=null;
                    DataTable schema = null;
                    List<DataTable> tables =view.getTables(); 
                    List<DataTable> schemas =view.getSchemas();

                    for(int i=0;i<tables.Count;++i)
                        if(tables[i].TableName==tableName)
                        {
                            table = tables[i];
                            schema = schemas[i];
                        }
                            

                    if(table!=null && schema!=null)
                        model.changeTableDisconnected(tableName, table, schema);
                    else 
                        throw new ArgumentException("Wrong table name.");

                    if (model.getForeignKeys().Count > 0)
                    {
                        this.extendedModel = new ExtendedDefaultTableModel(model.getConnection());
                        extendedModel.changeTable(tableName);
                    }
                    updateView();
                }
            }
               
        }

        public void updateAll()
        {
            updateModel();
            view.update(model.getData());
        }

        public DataColumn getIdentityColumn()
        {
            return model.getIdentityColumn();
        }

        public List<string> getTableNames()
        {
            if (overviewModel != null)
                return overviewModel.getTableNames();
            else
                return null;
        }

        public string getTableName()
        {
            return model.getTableName();
        }

        public DataColumn[] getPrimaryKeys()
        {
            return model.getPrimaryKeys();
        }

        public void insert(List<Object> values)
        {
            model.insert(values);
        }

        public List<string[]> getForeignKeys()
        {
            return model.getForeignKeys();
        }

        public DataColumnCollection getColumns()
        {
            return model.getColumns();
        }

        public DataColumnCollection getColumnsWithoutIdentity()
        {
            DataColumnCollection columns = model.getColumns();
            DataColumn identityColumn= getIdentityColumn();

            for (int i = 0; i < columns.Count; ++i)
                if (columns[i].ColumnName == identityColumn.ColumnName)
                    columns.RemoveAt(i);

            return columns;
        }

        public List<DataTable> getReferencedTablesData()
        {
            if (extendedModel != null)
                return extendedModel.getReferencedTablesData();
            else
                return null;
        }

        public void delete(List<int> indices)
        {
            if (!Program.connectedMode)
            {
                model.deleteDisconnected(indices);
                updateView();
            }
                
            else
            {
                model.deleteConnected(indices);
                updateAll();
            }
                

            
        }

        public void update()
        { 

        }

        public void sync()
        {
            model.sync();
        }

    }
}
