namespace DataGenerator.Services;

public interface IDataGeneratorService
{
    void GeneratePosData(int locationId, int customersCount);
    void DeletePosData(int locationId);
}