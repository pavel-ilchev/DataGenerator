namespace DataGenerator.Services.Interfaces;

using Models;

public interface IDatabaseSchemaService
{
    Dictionary<string, TableDto> GetSchema();
}