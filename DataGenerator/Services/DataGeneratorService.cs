namespace DataGenerator.Services;

using Microsoft.Data.SqlClient;
using Models;

public class DataGeneratorService : IDataGeneratorService
{
    private const string AutoIncrColName = "RowId";
    private readonly string connectionString;
    private readonly IFakeDataService fakeDataService;
    private readonly IDatabaseSchemaService schemaService;
    private SqlConnection connectionInstance;

    public DataGeneratorService(
        IDatabaseSchemaService schemaService,
        IConfiguration configuration,
        IFakeDataService fakeDataService)
    {
        this.schemaService = schemaService;
        this.fakeDataService = fakeDataService;
        this.connectionString = configuration.GetConnectionString("DestinationEntities");
    }

    private SqlConnection Connection
    {
        get
        {
            if (this.connectionInstance == null)
            {
                this.connectionInstance = new SqlConnection(this.connectionString);
                this.Connection.Open();
            }

            return this.connectionInstance;
        }
    }

    public void GeneratePosData(int locationId)
    {
        const int personsToGenerateCount = 5;
        var schema = this.schemaService.GetSchema();

        for (int i = 0; i < personsToGenerateCount; i++)
        {
            this.GenerateDataForCustomer(schema, locationId);
        }
    }

    private void GenerateDataForCustomer(Dictionary<string, TableDto> schema, int locationId)
    {
        var custId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var orderedTables = schema.OrderBy(a => a.Value.Order);
        foreach (var table in orderedTables)
        {
            this.InsertData(custId, orderId, table.Value, locationId);
        }
    }

    private void InsertData(Guid custId, Guid orderId, TableDto table, int locationId)
    {
        var tableColumns = table.Columns.Where(c => c.Name != AutoIncrColName).ToList();
        var columnNames = tableColumns.Select(c => c.Name).ToList();
        var columnValues = columnNames.Select(c => $"@{c}").ToList();
        string query =
            $"INSERT INTO {table.TableName} ({string.Join(",", columnNames)}) VALUES ({string.Join(",", columnValues)})";

        var command = new SqlCommand(query, this.Connection);
        foreach (var col in tableColumns)
        {
            object value;
            if (col.Name.Equals("custid", StringComparison.InvariantCultureIgnoreCase))
            {
                value = custId;
            }
            else if (col.Name.Equals("orderid", StringComparison.InvariantCultureIgnoreCase))
            {
                value = orderId;
            }
            else
            {
                value = this.fakeDataService.GetFakeData(col.Name, col.Type, col.StringMaxLenght, locationId);
            }

            command.Parameters.AddWithValue($"@{col.Name}", value);
        }

        command.ExecuteNonQuery();
    }
}