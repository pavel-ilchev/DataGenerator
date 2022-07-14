namespace DataGenerator.Services;

using Interfaces;
using KAPIClient;
using KAPIClient.Models;
using Microsoft.Extensions.Options;

public class KapiFactoryService : IKapiFactoryService
{
    private readonly IOptions<KapiAppSettings> kapiAppSettings;

    public KapiFactoryService(IOptions<KapiAppSettings> settings)
    {
        if (settings == null || settings.Value == null || string.IsNullOrEmpty(settings.Value.Domain) ||
            string.IsNullOrEmpty(settings.Value.OAuthProviderDomain) ||
            string.IsNullOrEmpty(settings.Value.OAuthProviderDomainClientId) ||
            string.IsNullOrEmpty(settings.Value.OAuthProviderDomainClientSecret))
        {
            throw new ArgumentNullException(nameof(KapiAppSettings));
        }

        this.kapiAppSettings = settings;
    }

    public IKAPI CreateKapi()
    {
        return new KAPI(this.kapiAppSettings.Value);
    }
}