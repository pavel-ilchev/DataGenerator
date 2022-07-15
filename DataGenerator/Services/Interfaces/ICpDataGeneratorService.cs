namespace DataGenerator.Services.Interfaces;

using Models;

public interface ICpDataGeneratorService
{
    Dictionary<string, TableDto> Generate(int clientId, int locationId, string locationName);
}