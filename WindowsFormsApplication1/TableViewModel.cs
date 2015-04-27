using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public interface TableViewModel
    {
        void updateView();
        void updateModel();
        void changeTable(string tableName);
        string getTableName();
        List<string> getTableNames();
        List<string[]> getForeignKeys();
        DataColumn[] getPrimaryKeys();
        DataColumnCollection getColumns();
    }

    class DefaultTableViewModel : TableViewModel
    {
        private TableModel model;
        private MainForm view;
        private DatabaseOverviewModel overviewModel;

        public DefaultTableViewModel(MainForm view, SqlConnection connection)
        {
            if (view == null || connection==null)
                throw new ArgumentNullException();
            this.view = view;
            this.model = new DefaultTableModel(connection);
            this.overviewModel = new DefaultDatabaseOverviewModel(connection);
        }

        public DefaultTableViewModel(MainForm view, TableModel model, DatabaseOverviewModel overviewModel)
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

        public void changeTable(string tableName)
        {
            model.changeTable(tableName);
            updateView();
        }

        public void updateView()
        {
            view.update(model.getData());
        }

        public List<string> getTableNames()
        {
            return overviewModel.getTableNames();
        }

        public string getTableName()
        {
            return model.getTableName();
        }

        public DataColumn[] getPrimaryKeys()
        {
            return model.getPrimaryKeys();
        }

        public List<string[]> getForeignKeys()
        {
            return model.getForeignKeys();
        }

        public DataColumnCollection getColumns()
        {
            return model.getColumns();
        }
    }
}
