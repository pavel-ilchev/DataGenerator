namespace DataGenerator.Services;

using System.Data;
using System.Data.OleDb;
using Interfaces;
using Microsoft.Data.SqlClient;
using Models;

public class DatabaseSchemaService : IDatabaseSchemaService
{
    public Dictionary<string, TableDto> GetSchema(string connectionString, List<string> tableOrder)
    {
        var result = new Dictionary<string, TableDto>();
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var data = connection.GetSchema("Tables");


        foreach (DataRow dataRow in data.Rows)
        {
            string catalog = dataRow["TABLE_CATALOG"].ToString();
            string tableSchema = dataRow["TABLE_SCHEMA"].ToString();
            string tableName = dataRow["TABLE_NAME"].ToString();
            if (string.IsNullOrEmpty(tableName) ||
                tableName.Equals("sysdiagrams", StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }

            var tableDto = new TableDto
            {
                Order = tableOrder.Contains(tableName) ? tableOrder.IndexOf(tableName) : int.MaxValue,
                TableName = $"[{tableSchema}].[{tableName}]",
                Columns = new List<ColumnDto>()
            };

            var tbl = new DataTable(tableName);
            var columnsTable = connection.GetSchema("Columns", new[] { null, null, tbl.TableName, null });
            foreach (DataRow row in columnsTable.Rows)
            {
                var col = new DataColumn(row["COLUMN_NAME"].ToString());
                string colName = col.ColumnName;

                //row _columns is mapping for row ItemArray 
                int ordinalPosition = int.Parse(row["ORDINAL_POSITION"].ToString());
                string colType = row["DATA_TYPE"].ToString();
                string colMaxLength = row["CHARACTER_MAXIMUM_LENGTH"].ToString();
                int numericPrecision = !string.IsNullOrEmpty(row["NUMERIC_PRECISION"].ToString())
                    ? int.Parse(row["NUMERIC_PRECISION"].ToString())
                    : 0;
                int numericScale = !string.IsNullOrEmpty(row["NUMERIC_SCALE"].ToString())
                    ? int.Parse(row["NUMERIC_SCALE"].ToString())
                    : 0;
                tableDto.Columns.Add(new ColumnDto
                {
                    OrdinalPosition = ordinalPosition,
                    Name = colName,
                    Type = colType,
                    StringMaxLength = colMaxLength,
                    NumericPrecision = numericPrecision,
                    NumericScale = numericScale
                });
            }

            // Sort columns in order so we can insert them
            tableDto.Columns = tableDto.Columns.OrderBy(c => c.OrdinalPosition).ToList();
            result.Add(tableName, tableDto);
        }

        return result;
    }

    public Dictionary<string, TableDto> GetDependencies(string connectionString, string connectionStringOle)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        const string pKCommandString = "select TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS " +
                                       "where TABLE_SCHEMA = 'dbo' and COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1 " +
                                       "order by TABLE_NAME";

        var dependenciesColumnsByTable = new Dictionary<string, DependenciesDto>();
        var dataSet = new DataSet();
        var sqlDataAdapter = new SqlDataAdapter();
        sqlDataAdapter.SelectCommand = new SqlCommand(pKCommandString, connection);
        sqlDataAdapter.Fill(dataSet);

        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                var dependencie = new DependenciesDto { PkColName = row[2] as string };
                dependenciesColumnsByTable.Add(row[1] as string, dependencie);
            }
        }

        const string fKCommandString = @"SELECT 
                                           OBJECT_NAME(f.parent_object_id) TableName,
                                           COL_NAME(fc.parent_object_id,fc.parent_column_id) ColName, 
                                           OBJECT_NAME (f.referenced_object_id) TableFkRefferTo
                                        FROM  sys.foreign_keys AS f
                                        INNER JOIN sys.foreign_key_columns AS fc ON f.OBJECT_ID = fc.constraint_object_id
                                        INNER JOIN sys.tables t ON t.OBJECT_ID = fc.referenced_object_id";

        using var connectionFk = new SqlConnection(connectionString);
        var dataSetFk = new DataSet();
        var sqlDataAdapterFk = new SqlDataAdapter();
        sqlDataAdapterFk.SelectCommand = new SqlCommand(fKCommandString, connectionFk);
        sqlDataAdapterFk.Fill(dataSetFk);

        if (dataSetFk.Tables.Count > 0 && dataSetFk.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow row in dataSetFk.Tables[0].Rows)
            {
                string tableName = (string)row[0];
                if (!dependenciesColumnsByTable.ContainsKey((string)row[0]))
                {
                    dependenciesColumnsByTable.Add(tableName, new DependenciesDto());
                }

                if (dependenciesColumnsByTable[tableName].FkTablesByColName == null)
                {
                    dependenciesColumnsByTable[tableName].FkTablesByColName = new Dictionary<string, string>();
                }

                string fkTableName = (string)row[2];
                string fkColName = (string)row[1];
                if (!dependenciesColumnsByTable[tableName].FkTablesByColName.ContainsKey(fkColName))
                {
                    dependenciesColumnsByTable[tableName].FkTablesByColName.Add(fkColName, fkTableName);
                }
            }
        }

        var result = new Dictionary<string, TableDto>();
        var oleConnection = new OleDbConnection(connectionStringOle);
        oleConnection.Open();
        string[] restrictions = { null };
        var dt = oleConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, restrictions);

        foreach (DataRow dataRow in dt.Rows)
        {
            // string catalog = dataRow["TABLE_CATALOG"].ToString();
            string tableSchema = dataRow["PK_TABLE_SCHEMA"].ToString();
            string tableName = dataRow["FK_TABLE_NAME"].ToString();
            if (string.IsNullOrEmpty(tableName) ||
                tableName.Equals("sysdiagrams", StringComparison.CurrentCultureIgnoreCase) ||
                result.ContainsKey(tableName))
            {
                continue;
            }

            var tableDto = new TableDto
            {
                TableName = $"[{tableSchema}].[{tableName}]",
                Columns = new List<ColumnDto>(),
                Dependencies = dependenciesColumnsByTable.ContainsKey(tableName)
                    ? dependenciesColumnsByTable[tableName]
                    : null
            };

            var tbl = new DataTable(tableName);
            // var columnsTableOle =
            //     oleConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new[] { null, null, tbl.TableName, null });
            var columnsTable = connection.GetSchema("Columns", new[] { null, null, tbl.TableName, null });
            foreach (DataRow row in columnsTable.Rows)
            {
                var col = new DataColumn(row["COLUMN_NAME"].ToString());
                string colName = col.ColumnName;

                //row _columns is mapping for row ItemArray 
                int ordinalPosition = int.Parse(row["ORDINAL_POSITION"].ToString());
                string colType = row["DATA_TYPE"].ToString();
                string colMaxLength = row["CHARACTER_MAXIMUM_LENGTH"].ToString();
                int numericPrecision = !string.IsNullOrEmpty(row["NUMERIC_PRECISION"].ToString())
                    ? int.Parse(row["NUMERIC_PRECISION"].ToString())
                    : 0;
                int numericScale = !string.IsNullOrEmpty(row["NUMERIC_SCALE"].ToString())
                    ? int.Parse(row["NUMERIC_SCALE"].ToString())
                    : 0;


                tableDto.Columns.Add(new ColumnDto
                {
                    OrdinalPosition = ordinalPosition,
                    Name = colName,
                    Type = colType,
                    StringMaxLength = colMaxLength,
                    NumericPrecision = numericPrecision,
                    NumericScale = numericScale
                });
            }

            result.Add(tableName, tableDto);
        }

        return result;
    }
}