namespace DataGenerator.Services;

using Interfaces;
using Microsoft.Data.SqlClient;
using Models;

public class CpDataGeneratorService : ICpDataGeneratorService
{
    private readonly SqlConnection connectionInstance;
    private readonly string connectionString;
    private readonly string connectionStringOle;
    private readonly IFakeDataService fakeDataService;
    private readonly List<string> notPopulatedTables = new() { "Client", "Locations", "CountriesPhoneCodes" };
    private readonly Dictionary<string, List<object>> pksByPopulatedTables = new();
    private readonly IDatabaseSchemaService schemaService;
    private readonly List<string> tablesToPopulate = new() { "TwilioVoiceRequests", "Appointment" };


    public CpDataGeneratorService(
        IConfiguration configuration,
        IDatabaseSchemaService schemaService,
        IFakeDataService fakeDataService)
    {
        this.connectionString = configuration.GetConnectionString("CpEntities");
        this.connectionStringOle = configuration.GetConnectionString("CpEntitiesOle");
        this.schemaService = schemaService;
        this.fakeDataService = fakeDataService;
        this.connectionInstance = new SqlConnection(this.connectionString);
        this.connectionInstance.Open();
    }

    public Dictionary<string, TableDto> Generate(int clientId, int locationId, string locationName)
    {
        // var schema = this.schemaService.GetSchema(this.connectionString, new List<string>());
        var schemaOle = this.schemaService.GetDependencies(this.connectionString, this.connectionStringOle);

        var tables = schemaOle.Where(t => this.tablesToPopulate.Contains(t.Key)).ToList();
        int count = 0;
        while (count < tables.Count)
        {
            foreach (var table in tables)
            {
                bool success = this.PopulateTable(table.Value, clientId, locationId, locationName);
                if (success)
                {
                    count++;
                }
            }
        }

        return schemaOle;
    }

    private bool PopulateTable(TableDto table, int clientId, int locationId, string locationName)
    {
        if (table.Dependencies != null && table.Dependencies.FkTablesByColName != null)
        {
            var dependantTables = table.Dependencies.FkTablesByColName.Values;
            var populatedTables = this.pksByPopulatedTables.Keys;
            foreach (string dependantTable in dependantTables)
            {
                if (!populatedTables.Contains(dependantTable) && !this.notPopulatedTables.Contains(dependantTable))
                {
                    return false;
                }
            }
        }

        this.InsertData(table, clientId, locationId, locationName);

        return true;
    }

    private void InsertData(TableDto table, int clientId, int locationId, string locationName)
    {
        var tableColumns = table.Columns.Where(c => c.Name != table.Dependencies.PkColName).ToList();
        var columnNames = tableColumns.Select(c => $"[{c.Name}]").ToList();
        var columnValues = tableColumns.Select(c => c.Name).Select(c => $"@{c}").ToList();
        string query =
            $"INSERT INTO {table.TableName} ({string.Join(",", columnNames)}) VALUES ({string.Join(",", columnValues)})";

        var command = new SqlCommand(query, this.connectionInstance);
        foreach (var col in tableColumns)
        {
            object value;
            if (table.Dependencies?.FkTablesByColName != null &&
                table.Dependencies.FkTablesByColName.ContainsKey(col.Name) &&
                table.Dependencies.FkTablesByColName[col.Name] == "Location")
            {
                value = locationId;
            }
            else if (table.Dependencies?.FkTablesByColName != null &&
                     table.Dependencies.FkTablesByColName.ContainsKey(col.Name) &&
                     table.Dependencies.FkTablesByColName[col.Name] == "Client")
            {
                value = clientId;
            }
            else if (col.Name.Equals("locationname", StringComparison.InvariantCultureIgnoreCase))
            {
                value = locationName;
            }
            else
            {
                value = this.fakeDataService.GetFakeData(col.Name, col.Type, col.StringMaxLength, locationId, 0);
            }

            command.Parameters.AddWithValue($"@{col.Name}", value ?? DBNull.Value);
        }

        command.ExecuteNonQuery();
    }
}