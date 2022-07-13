namespace DataGenerator.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly IDatabaseSchemaService databaseSchemaService;
    private readonly IDataGeneratorService generatorService;

    public TestController(IDatabaseSchemaService databaseSchemaService, IDataGeneratorService generatorService)
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