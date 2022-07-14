namespace DataGenerator.Services;

using Database;
using Interfaces;
using KAPIClient;
using KAPIClient.Exceptions;
using KAPIClient.Models;

public class ClientCreatorService : IClientCreatorService
{
    private readonly CpContext context;
    private readonly IKAPI kapi;

    public ClientCreatorService(CpContext context, IKAPI kapi)
    {
        this.context = context;
        this.kapi = kapi;
    }

    public void CreateClient(string clientName, int locationsCount, Guid aspnetId)
    {
        int clientId = this.CreateClientInDatabase(clientName, aspnetId);
        this.CreateClientInKapi(clientName, clientId);
    }

    private void CreateClientInKapi(string clientName, int clientId)
    {
        Client kapiClient = null;

        try
        {
            kapiClient = this.kapi.Client.GetClient(clientId);
        }
        catch (KapiException)
        {
            // Expected behaviour. This checks if client is already created and throws exception if it is not.
        }

        if (kapiClient == null)
        {
            var dataClient = new ClientAddRequest { CpClientId = clientId, CpId = "01", Name = clientName };

            this.kapi.Client.AddClient(dataClient);
        }
    }

    private int CreateClientInDatabase(string clientName, Guid aspnetId)
    {
        var client = new Database.CpEntities.Client
        {
            Aspnetid = aspnetId,
            Name = clientName,
            Phone = $"(555) {FakeDataService.RandomNumber(3)}-{FakeDataService.RandomNumber(4)}",
            Units = "Miles",
            PlatformVersion = 2,
            IndustryId = 1
        };

        this.context.Clients.Add(client);
        this.context.SaveChanges();

        return client.Id;
    }
}