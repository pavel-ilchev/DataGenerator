using DataGenerator.Database;
using DataGenerator.Services;
using DataGenerator.Services.Interfaces;
using KAPIClient.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CpContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("CpEntities")));

builder.Services.AddScoped<IDatabaseSchemaService, DatabaseSchemaService>();
builder.Services.AddScoped<IDataGeneratorService, DataGeneratorService>();
builder.Services.AddScoped<IFakeDataService, FakeDataService>();
builder.Services.AddScoped<IClientCreatorService, ClientCreatorService>();

builder.Services.Configure<KapiAppSettings>(builder.Configuration.GetSection("Kapi:AppSettings"));
builder.Services.AddSingleton<IKapiFactoryService, KapiFactoryService>();
builder.Services.AddSingleton(provider => provider.GetService<IKapiFactoryService>().CreateKapi());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();