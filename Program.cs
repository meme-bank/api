using GraphQL;
using OctopusAPI.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MeduzaContext>();
// builder.Services.AddControllers();
// builder.Services.AddGraphQL(
//   b => b
//     .AddAutoSchema<MeduzaSchema>()
//     .AddSystemTextJson()
//     .AddDataLoader()
// );

var app = builder.Build();

app.UseHttpsRedirection();
// app.UseGraphQLAltair("/ui/altair");

app.Run();