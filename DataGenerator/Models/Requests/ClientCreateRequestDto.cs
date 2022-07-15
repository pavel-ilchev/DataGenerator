namespace DataGenerator.Models.Requests;

public class ClientCreateRequest
{
    public string ClientName { get; set; }
    public int LocationsCount { get; set; }
    public Guid AspnetId { get; set; }
}