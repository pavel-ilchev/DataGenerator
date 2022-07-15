namespace DataGenerator.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

[ApiController]
[Route("[controller]")]
public class GeneratorController : ControllerBase
{
    private readonly ICpDataGeneratorService cpGeneratorService;
    private readonly IPosDataGeneratorService generatorService;

    public GeneratorController(IPosDataGeneratorService generatorService, ICpDataGeneratorService cpGeneratorService)
    {
        this.generatorService = generatorService;
        this.cpGeneratorService = cpGeneratorService;
    }

    [HttpGet]
    public IActionResult Generate(int locationId, int customersCount = 5)
    {
        this.generatorService.GeneratePosData(locationId, customersCount);

        return this.Ok();
    }


    [HttpGet("cp")]
    public IActionResult GenerateCpData(int clientId, int locationId, string locationName)
    {
        var result = this.cpGeneratorService.Generate(clientId, locationId, locationName);

        return this.Ok(result);
    }

    [HttpGet("wipe")]
    public IActionResult Wipe(int locationId)
    {
        this.generatorService.DeletePosData(locationId);

        return this.Ok();
    }
}