namespace DataGenerator.Services.Interfaces;

using Models;

public interface IDatabaseSchemaService
{
    Dictionary<string, TableDto> GetSchema(string connectionString, List<string> tableOrder);
    Dictionary<string, TableDto> GetDependencies(string connectionString, string connectionStringOle);
}