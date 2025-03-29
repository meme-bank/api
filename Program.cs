using OctopusAPI.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MeduzaContext>();

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();