using babbly_like_service.Repositories;
using babbly_like_service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Cassandra settings
var cassandraSection = builder.Configuration.GetSection("Cassandra");
builder.Services.Configure<CassandraSettings>(options =>
{
    options.Hosts = builder.Configuration.GetValue<string>("CassandraHosts")?.Split(',') ?? new[] { "localhost" };
    options.Keyspace = builder.Configuration.GetValue<string>("CassandraKeyspace") ?? "babbly_likes";
    options.Username = builder.Configuration.GetValue<string>("CassandraUsername") ?? string.Empty;
    options.Password = builder.Configuration.GetValue<string>("CassandraPassword") ?? string.Empty;
});

// Register services
builder.Services.AddSingleton<CassandraService>();
builder.Services.AddScoped<ILikeRepository, CassandraLikeRepository>();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
