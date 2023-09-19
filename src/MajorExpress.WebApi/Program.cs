using MajorExpress.Infrastructure;
using MajorExpress.Infrastructure.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
       .UseSwaggerUI();
}

await app.MigrateDatabase(app.Lifetime.ApplicationStopping);

app.UseHttpsRedirection()
   .UseAuthorization();

app.MapControllers();

app.Run();
