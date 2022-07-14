namespace DataGenerator.Services.Interfaces;

using KAPIClient;

public interface IKapiFactoryService
{
    public IKAPI CreateKapi();
}