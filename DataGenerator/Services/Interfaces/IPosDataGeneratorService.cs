namespace DataGenerator.Services.Interfaces;

public interface IPosDataGeneratorService
{
    void GeneratePosData(int locationId, int customersCount);
    void DeletePosData(int locationId);
}