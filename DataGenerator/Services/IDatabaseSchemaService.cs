namespace DataGenerator.Services;

using Models;

public interface IDatabaseSchemaService
{
    Dictionary<string, TableDto> GetSchema();
}