namespace DataGenerator.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

[ApiController]
[Route("[controller]")]
public class CreateClientController : ControllerBase
{
    private readonly IClientCreatorService clientService;

    public CreateClientController(IClientCreatorService clientService)
    {
        this.clientService = clientService;
    }

    [HttpPost]
    public IActionResult CreateClient(string clientName, int locationsCount, Guid aspnetId)
    {
        this.clientService.CreateClient(clientName, locationsCount, aspnetId);

        return this.Ok();
    }
}