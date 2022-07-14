namespace DataGenerator.Services;

using System.Data;
using Microsoft.Data.SqlClient;
using Models;

public class DatabaseSchemaService : IDatabaseSchemaService
{
    private readonly string connectionString;
    private readonly string connectionStringOle;
    private readonly List<string> tableForPopulating = new() { "Client", "Locations", "SMS_Log" };
    private readonly List<string> tableOrder = new() { "Person", "Orders" };

    public DatabaseSchemaService(IConfiguration configuration)
    {
        this.connectionString = configuration.GetConnectionString("Entities");
        this.connectionStringOle = configuration.GetConnectionString("EntitiesOle");
    }

    public Dictionary<string, TableDto> GetSchema()
    {
        //this.CopyDatabase();
        var result = new Dictionary<string, TableDto>();
        using var connection = new SqlConnection(this.connectionString);
        connection.Open();
        var data = connection.GetSchema("Tables");

        //Get dependancies
        // var oleConnection = new OleDbConnection(this.connectionStringOle);
        // oleConnection.Open();
        // string[] restrictions = { null };
        // var dt = oleConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, restrictions);


        foreach (DataRow dataRow in data.Rows)
        {
            string catalog = dataRow["TABLE_CATALOG"].ToString();
            string tableSchema = dataRow["TABLE_SCHEMA"].ToString();
            string tableName = dataRow["TABLE_NAME"].ToString();
            if (string.IsNullOrEmpty(tableName) ||
                // !this.tableForPopulating.Contains(tableName) ||
                tableName.Equals("sysdiagrams", StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }

            var tableDto = new TableDto
            {
                Order = this.tableOrder.Contains(tableName) ? this.tableOrder.IndexOf(tableName) : int.MaxValue,
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
}