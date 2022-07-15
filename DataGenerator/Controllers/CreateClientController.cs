using DataGenerator.Models.Requests;

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
    public IActionResult CreateClient(ClientCreateRequest request)
    {
        this.clientService.CreateClient(request.ClientName, request.LocationsCount, request.AspnetId);

        return this.Ok();
    }
}