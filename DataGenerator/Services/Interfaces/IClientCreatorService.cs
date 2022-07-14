namespace DataGenerator.Services.Interfaces;

public interface IClientCreatorService
{
    void CreateClient(string clientName, int locationsCount, Guid aspnetId);
}