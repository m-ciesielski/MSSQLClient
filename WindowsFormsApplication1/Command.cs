using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace WindowsFormsApplication1
{
    class Command
    {
        /**
 * Class Command creates SQL Insert, Update and Delete statements
 * as strings ready to execute by Statement.execute() method. 
 *
 * TODO: constructors unification
 */
    public enum MainStatement{UPDATE, INSERT, DELETE};
	protected String tableName;
	protected String condition;
	/**
	 * Defines which column should be updated on Update command.
	 */
	protected String updateColumn;
	//private ArrayList<String> constraints;
	protected List<Object> values;
	protected List<String> columns;
    protected MainStatement mainStat;

	
	//TODO: constructor renaming?
	
	//DELETE
	public Command(MainStatement mainStat, String tableName, List<String> columns, int [] condition)
	{
		if(tableName==null || columns==null || condition ==null)
			throw new ArgumentException("None of Command constructor arguments can be null.");
		if(columns.Count>condition.Length)
			throw new ArgumentException("Number of composite primary key columns cannot exceed the number of provided primary keys.");
		if(columns.Count==0 ||condition.Length==0)
			throw new ArgumentException("At least one column name and condition value must be provided.");
		this.mainStat=mainStat;
		this.tableName=tableName;
		this.columns=columns;
		if(columns.Count==1)
			this.condition=singularKeyConditionToString(condition);
		else
			this.condition=compositeKeyConditionToString(condition);
	}
	
	//INSERT
	
	/**
	 * Creates insert command. Count of columns must be equal to the count of values.
	 * @param mainStat
	 * @param tableName
	 * @param columns
	 * @param values
	 */
	public Command(MainStatement mainStat, String tableName,List<String> columns,List<Object> values){
		if(tableName==null || columns==null || values==null)
			throw new ArgumentException("None of Command constructor arguments can be null.");
		if(columns.Count!=values.Count)
			throw new ArgumentException("Count of columns must be equal to the count of values.");
		if(columns.Count==0 && values.Count==0)
			throw new ArgumentException("Count of columns and values must be greater than zero.");
		this.mainStat=mainStat;
		this.tableName=tableName;
		this.columns=columns;
		this.condition="";
		this.values=values;
	}

    //INSERT

    /**
     * Creates insert command. Count of columns must be equal to the count of values.
     * @param mainStat
     * @param tableName
     * @param columns
     * @param values
     */
    public Command(MainStatement mainStat, String tableName, DataColumnCollection columns, List<Object> values)
    {
        if (tableName == null || columns == null || values == null)
            throw new ArgumentException("None of Command constructor arguments can be null.");
        if (columns.Count != values.Count)
            throw new ArgumentException("Count of columns must be equal to the count of values.");
        if (columns.Count == 0 && values.Count == 0)
            throw new ArgumentException("Count of columns and values must be greater than zero.");
        this.mainStat = mainStat;
        this.tableName = tableName;

        this.columns=new List<string>();
        for (int i = 0; i < columns.Count; ++i)
            this.columns.Add(columns[i].ColumnName);

        this.condition = "";
        this.values = values;
    }
	
	//UPDATE
	public Command(MainStatement mainStat, String tableName, List<String> columns,
			int [] condition, Object value, String updateColumn) {
		if(tableName==null || columns==null || condition==null || value==null || updateColumn==null)
			throw new ArgumentException("None of Command constructor arguments can be null.");
		//TODO:length check
		if(condition.Length==0)
			throw new ArgumentException("Condition primary keys must be provided.");
		this.mainStat=mainStat;
		this.tableName=tableName;
		this.columns=columns;
		this.updateColumn=updateColumn;
		if(columns.Count==1)
			this.condition=singularKeyConditionToString(condition);
		else
			this.condition=compositeKeyConditionToString(condition);
		values=new List<Object>();
		this.values.Add(value);
	}
	
	

	/**
	 * Creates a condition of Delete/Update SQL statement by joining given primary keys
	 * with OR operator.  It can be used only with tables that utilize singular primary key.
	 * 
	 * @param condition an array consisting of primary keys of rows that should be deleted
	 * @return string that describes deletion condition 
	 * 
	 */
	protected String singularKeyConditionToString(int [] condition){
		StringBuilder buff = new StringBuilder();
		buff.Append(columns[0]+ " = ");
		for(int i=0;i<condition.Length;++i)
			buff.Append(condition[i]+ " OR ");
		
        
		return buff.ToString().Substring(0, buff.Length-3);
	}
	/**
	 * Creates a condition of Delete/Update SQL statement. Primary keys in constraint array
	 * must be organized in the following schema: indices of keys of 
	 * first composite primary key column must satisfy equation index % constraint.length == 0,
	 * indices of second column must satisfy equation index % constraint.length == 1, and so on.
	 * 
	 *  It can be used only with tables that utilize composite primary key.
	 * 
	 * @param condition an array consisting of primary keys of rows that should be deleted
	 * @return string that describes deletion condition 
	 * 
	 */
	protected String compositeKeyConditionToString(int[] condition)
	{
		int rowsToDelete=condition.Length/columns.Count;
		
		StringBuilder buff = new StringBuilder();
		
		for(int i=0;i<rowsToDelete;++i)
		{
			buff.Append("( ");
			for(int j=0;j<columns.Count;++j)
			{
				buff.Append(columns[j]+" = "+condition[j+i*columns.Count]+" AND " );
			}
			buff.Length=buff.Length-4;
			buff.Append(") OR ");
		}
		
		return buff.ToString().Substring(0, buff.Length-3);
	}
	
	protected String columnArrayListToString(){
		StringBuilder buff;
		buff=new StringBuilder();
		for(int i=0;i<columns.Count;++i)
			buff.Append(columns[i]+", ");

		
		return buff.ToString().Substring(0, buff.Length-2);
		
	}
	
	protected String valuesArrayListToString()
	{
		StringBuilder buff;
		buff=new StringBuilder();
		for(int i=0;i<values.Count;++i)
		{
			if(values[i].GetType()==typeof(Boolean))
			{
				if((Boolean)values[i]==true)
					buff.Append(1+", ");
				else
					buff.Append(0+", ");
			}
			else if(values[i].GetType()==typeof(string))
				buff.Append("\""+values[i]+"\""+", ");
			else
			buff.Append(values[i]+", ");
		}



        return buff.ToString().Substring(0, buff.Length - 2);
	}

    protected string MainStatementToString()
    {
        switch (mainStat)
        {
            case MainStatement.DELETE:
                    return "DELETE FROM";

            case MainStatement.INSERT:
                    return "INSERT INTO";
            case MainStatement.UPDATE:
                    return "UPDATE";
            default:
                return "";

        }
    }

	public String createCommandString(){
		String CommandString = null;
		switch(mainStat){
			case MainStatement.DELETE:
			{
				
				CommandString=MainStatementToString()+" "+tableName+" WHERE "+condition;
				break;
			}
            case MainStatement.INSERT:
			{
                CommandString = MainStatementToString() + " " + tableName + " (" + columnArrayListToString() + ")" + " VALUES " + " (" +
						valuesArrayListToString()+")";
				break;
			}
            case MainStatement.UPDATE:
			{
                CommandString = MainStatementToString() + " " + tableName + " SET " + updateColumn + "=" + valuesArrayListToString() + " WHERE " + condition;
				break;
			}
			default:
				break;
			
		}
		return CommandString;
	}
}
    
}
