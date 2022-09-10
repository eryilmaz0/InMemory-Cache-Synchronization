using DistributedCache;
using DistributedCache.BackgroundService;
using DistributedCache.Cache;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheProvider, InMemoryCacheProvider>();
builder.Services.AddSingleton<ICacheSyncronizer, RedisCacheSyncronizer>();
builder.Services.AddHostedService<CacheSyncronizerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();