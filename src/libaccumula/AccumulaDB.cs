using System;
using System.IO;
using System.Threading.Tasks;

using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace libaccumula
{
    public class TableRows {
        public string schema { get; set; }
        public string table { get; set; }
        public DataTable data;
    }

    public class AccumulaDB
    {
        SqlConnectionStringBuilder ConnectionInfo = new SqlConnectionStringBuilder();

        public AccumulaDB()
        {
            ConnectionInfo.DataSource = "almacenes-sottaycia.database.windows.net"; 
            ConnectionInfo.UserID = "admin_almacenes";            
            ConnectionInfo.Password = "c3b!7jFnw9RjUy4-";     
            ConnectionInfo.InitialCatalog = "accumula";
        }

        public async Task AddRows(TableRows inputTable)
        {
            try 
            {
                string regex = "^[a-zA-Z0-9_-]+$";
                if ( ! Regex.Match(inputTable.schema, regex).Success
                    || ! Regex.Match(inputTable.table, regex).Success)
                
                {
                    throw new Exception($"Invalid characters in table name [{inputTable.schema}].[{inputTable.table}]. Use only [a-zA-Z0-9_-]");
                } 
                var sqlTable = await GetTable(inputTable.schema, inputTable.table);
                if (sqlTable.Columns.Count == 0)
                {
                    throw new Exception($"Schema query on table [{inputTable.schema}].[{inputTable.table}] returned no columns. Check if table exists");
                }

                CopyRows(inputTable.data, sqlTable);
                await InsertRows(sqlTable);
            }
            catch(Exception e)
            {
                var msg = $"Error in AddRows table '{inputTable.schema}.{inputTable.table}'";
                throw new Exception(msg, e);
            }
        }

        async Task InsertRows(DataTable sqlTable)
        {
            //TODO write to sql
            try 
            {
#if DEBUG
                foreach(DataRow r in sqlTable.Rows) foreach(DataColumn c in sqlTable.Columns) 
                {
                    Console.WriteLine($"{c.ColumnName} = {r[c.ColumnName]}");                    
                }
#endif
                using (SqlConnection connection = new SqlConnection(ConnectionInfo.ConnectionString))
                {
                    var table = sqlTable.TableName.Split('.');
                    String sql = $"usp_Insert_{table[0]}_{table[1]}";

                    await connection.OpenAsync();       

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@rows", sqlTable);                        
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException e)
            {
                var msg = $"Error in InsertRows table '{sqlTable.TableName}'";
                throw new Exception(msg, e);
                //Console.WriteLine(e.ToString());
            }            
        }

        async Task<DataTable> GetTable(string schema, string table)
        {
            try 
            { 
                DataTable result = new DataTable();
                result.TableName = $"{schema}.{table}";

                using (SqlConnection connection = new SqlConnection(ConnectionInfo.ConnectionString))
                {
                    string sql = @$"SELECT COLUMN_NAME, DATA_TYPE 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = '{table}' and TABLE_SCHEMA = '{schema}'";
                    Console.WriteLine(sql);

                    string[] ignoreColumns = { "CreatedOn", "Id" };

                    await connection.OpenAsync();       

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {                        
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var colName = reader["COLUMN_NAME"].ToString();
                                if (ignoreColumns.Any(c => c == colName)) continue;    

                                Console.WriteLine("{0} {1}", reader["COLUMN_NAME"], reader["DATA_TYPE"]);
                                Type dtType = null;
                                switch(reader["DATA_TYPE"])
                                {
                                    case "varchar":
                                    case "char":
                                        dtType = typeof(string);
                                        break;
                                    case "int":
                                        dtType = typeof(int);                                        
                                        break;
                                    case "decimal":
                                        dtType = typeof(decimal);                                        
                                        break;
                                    case "date":
                                    case "datetime":
                                        dtType = typeof(DateTime);
                                        break;
                                }
                                result.Columns.Add(colName, dtType);
                            }
                        }
                    }
                }
                return result;                    
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                var msg = $"Error in GetTable table '{schema}.{table}'";
                throw new Exception(msg, e);
            }            
        }

        void CopyRows(DataTable inputRows, DataTable outputTable)
        {
            foreach(DataRow inputRow in inputRows.Rows)
            {
                var outRow = outputTable.NewRow();
                foreach(DataColumn c in outputTable.Columns) 
                {
                    if (inputRows.Columns.Contains(c.ColumnName))
                    {
                        outRow[c.ColumnName] = inputRow[c.ColumnName];
                    }
                    else
                    {
                        outRow[c.ColumnName] = null;
                        Console.WriteLine($"{c.ColumnName} missing.");
                    }
                }
                outputTable.Rows.Add(outRow);
            }            
        }
    }

}
