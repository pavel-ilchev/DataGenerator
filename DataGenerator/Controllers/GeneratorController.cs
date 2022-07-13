namespace DataGenerator.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services;

[ApiController]
[Route("[controller]")]
public class GeneratorController : ControllerBase
{
    private readonly IDatabaseSchemaService databaseSchemaService;
    private readonly IDataGeneratorService generatorService;

    public GeneratorController(IDatabaseSchemaService databaseSchemaService, IDataGeneratorService generatorService)
    {
        this.databaseSchemaService = databaseSchemaService;
        this.generatorService = generatorService;
    }

    [HttpGet]
    public IActionResult Generate(int locationId)
    {
        this.generatorService.GeneratePosData(locationId);

        return this.Ok();
    }
}