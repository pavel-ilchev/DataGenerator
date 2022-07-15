namespace DataGenerator.Services;

using System.Text.RegularExpressions;
using Database;
using Database.CpEntities;
using Faker;
using Interfaces;
using KAPIClient;
using KAPIClient.Exceptions;
using KAPIClient.Models;
using Client = Database.CpEntities.Client;
using Location = Database.CpEntities.Location;

public class ClientCreatorService : IClientCreatorService
{
    private readonly CpContext cpContext;
    private readonly IPosDataGeneratorService dataGeneratorService;

    private readonly IKAPI kapi;
    private readonly PosUpdateContext posUpdateContext;

    public ClientCreatorService(
        CpContext cpContext,
        PosUpdateContext posUpdateContext,
        IKAPI kapi,
        IPosDataGeneratorService dataGeneratorService)
    {
        this.cpContext = cpContext;
        this.posUpdateContext = posUpdateContext;
        this.kapi = kapi;
        this.dataGeneratorService = dataGeneratorService;
    }

    public void CreateClient(string clientName, int locationsCount, Guid aspnetId)
    {
        var client = this.CreateClientInDatabase(clientName, aspnetId);
        this.CreateClientInPosupdate(client);
        this.CreateClientInKapi(clientName, client.Id);
        for (int i = 0; i < locationsCount; i++)
        {
            string locationName = locationsCount > 1 ? $"{clientName} - {i + 1}" : clientName;
            var location = this.CreateLocationInDatabase(client.Id, locationName);
            var locationInfo = this.CreateLocationInfoInDatabase(location);
            this.CreateLocationInPosupdate(location);
            this.CreateClientLocationInDatabase(location, locationInfo);
            this.CreateLocationInKapi(location, locationInfo);
            this.dataGeneratorService.GeneratePosData(location.Id, 10);
        }
    }

    private Client CreateClientInDatabase(string clientName, Guid aspnetId)
    {
        var client = new Client
        {
            Aspnetid = aspnetId,
            Name = clientName,
            Phone = FakeDataService.RandomPhoneNumber(),
            Units = "Miles",
            PlatformVersion = 2,
            IndustryId = 1,
            Url = Internet.DomainUrl(),
            ClientPath = Regex.Replace(Regex.Replace(clientName, "[\\\\/]", "-"), @"[^0-9a-zA-Z\._]", string.Empty)
                .Replace(".", string.Empty)
        };

        this.cpContext.Clients.Add(client);
        this.cpContext.SaveChanges();

        return client;
    }

    private Location CreateLocationInDatabase(int clientId, string locationName)
    {
        var location = new Location
        {
            ClientId = clientId,
            Name = locationName,
            Guid = Guid.NewGuid(),
            Posid = 120,
            TimeZoneId = 11,
            PhoneAreaCode = "601",
            LocAddress = Address.StreetName(),
            LocCity = Address.USCity(),
            LocRegion = Address.StateAbbreviation(),
            LocPostCode = Address.USZipCode(),
            LocCountryAbbreviation = "US"
        };

        this.cpContext.Locations.Add(location);
        this.cpContext.SaveChanges();

        return location;
    }

    private LocationsInfo CreateLocationInfoInDatabase(Location location)
    {
        var locationInfo = new LocationsInfo
        {
            ClientId = location.ClientId.Value,
            LocationId = location.Id,
            Lat = GeoLocation.Latitude(),
            Lng = GeoLocation.Longitude(),
            LeadEmail = User.Email(),
            DateAdded = DateTime.Now,
            TrackingPhone = FakeDataService.RandomPhoneNumber()
        };

        this.cpContext.Add(locationInfo);
        this.cpContext.SaveChanges();

        return locationInfo;
    }

    private void CreateClientLocationInDatabase(Location location, LocationsInfo locationInfo)
    {
        var clientLocation = new ClientLocation
        {
            ClientId = location.ClientId.Value,
            Country = location.LocCountryAbbreviation,
            Phone = locationInfo.TrackingPhone,
            City = location.LocCity,
            State = location.LocRegion,
            Email = locationInfo.LeadEmail,
            Zip = location.LocPostCode
        };

        this.cpContext.Add(clientLocation);
        this.cpContext.SaveChanges();
    }

    private void CreateClientInKapi(string clientName, int clientId)
    {
        KAPIClient.Models.Client kapiClient = null;

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

    private void CreateLocationInKapi(Location location, LocationsInfo locationInfo)
    {
        try
        {
            var worktimeKAPI = this.GenerateDefaultWorktime();
            var leadEmails = new List<string> { locationInfo.LeadEmail };
            var reviewProviders = new List<ReviewProvider>();

            var dataLocation = new KAPIClient.Models.Location
            {
                CpClientId = location.ClientId.Value,
                CpLocationId = location.Id,
                Name = location.Name,
                Street = location.LocAddress,
                City = location.LocCity,
                Region = location.LocRegion,
                PostalCode = location.LocPostCode,
                Country = location.LocCountryAbbreviation,
                WorkTime = worktimeKAPI,
                LeadEmail = leadEmails,
                ReviewProviders = reviewProviders,
                Telephone = FakeDataService.RandomPhoneNumber(),
                Latitude = locationInfo.Lat.Value,
                Longitude = locationInfo.Lng.Value
            };

            this.kapi.Location.AddLocation(dataLocation);
        }
        catch (Exception ex)
        {
        }
    }

    private void CreateClientInPosupdate(Client client)
    {
        var posupdateClient = new Database.PosUpdateEntities.Client
        {
            Aspnetid = client.Aspnetid,
            Cp = 1,
            Name = client.Name,
            ClientId = client.Id,
            PlatformVersion = client.PlatformVersion
        };

        this.posUpdateContext.Add(posupdateClient);
        this.posUpdateContext.SaveChanges();
    }

    private void CreateLocationInPosupdate(Location location)
    {
        var posupdateLocation = new Database.PosUpdateEntities.Location
        {
            Cp = 1,
            ClientId = location.ClientId,
            LocationId = location.Id,
            Name = location.Name,
            Guid = location.Guid,
            Posid = location.Posid,
            TimeZoneId = location.TimeZoneId,
            Country = "US",
            PosSubVersionId = 34
        };

        this.posUpdateContext.Add(posupdateLocation);
        this.posUpdateContext.SaveChanges();
    }

    private List<WorkTime> GenerateDefaultWorktime()
    {
        return Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().Select(d =>
            new WorkTime { Day = d.ToString(), OpenTime = "07:30 AM", CloseTime = "07:30 PM" }).ToList();
    }
}