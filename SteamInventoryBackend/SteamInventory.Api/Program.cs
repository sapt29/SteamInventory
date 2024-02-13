using Microsoft.OpenApi.Models;
using SteamInventory.BackgroundService;
using SteamInventory.Interfaces;
using SteamInventory.Repository.SteamItems;
using SteamInventory.Infrastructure.SteamWebApi;
using SteamInventory.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Steam Inventory API", Version = "1.0.0" });
    c.EnableAnnotations();
});

builder.Services.AddSingleton<IMainService,MainService>();
builder.Services.AddHostedService<DataBaseUpdateService>();
builder.Services.AddOptions<SteamWebApiConfiguration>().BindConfiguration(nameof(SteamWebApiConfiguration));
builder.Services.Configure<MongoDbConfiguration>(builder.Configuration.GetSection("MongoDbConfiguration"));
builder.Services.AddTransient<ISteamItemsRepository, SteamItemsMongoDbRepository>();
builder.Services.AddTransient<ISteamWebApi, SteamWebApi>();
var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Steam Inventory API");
});
app.MapControllers();

app.Run();
