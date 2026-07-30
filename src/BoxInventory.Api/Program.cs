using BoxInventory.Api.Extensions;
using BoxInventory.Api.Middleware;
using BoxInventory.Application;
using BoxInventory.Infrastructure;
using BoxInventory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Jwt:Authority"];
        options.Audience = builder.Configuration["Jwt:Audience"];
        options.RequireHttpsMetadata = bool.Parse(builder.Configuration["Jwt:RequireHttpsMetadata"] ?? "true");

        options.TokenValidationParameters.ValidateIssuer = true;
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidateLifetime = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddCorsPolicy(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();

app.UseSwaggerDocumentation();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.MapHealthCheckEndpoints();

// Ensure MongoDB indexes on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    await dbContext.EnsureIndexesAsync();
}

await app.RunAsync();
