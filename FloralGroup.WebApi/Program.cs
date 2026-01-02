using System.Security.Claims;
using System.Text;
using FloralGroup.Application.Services;
using FloralGroup.Domain.Interfaces;
using FloralGroup.Infrastructure.DataBaseModel; // Your DbContext namespace
using FloralGroup.Infrastructure.Repositories;
using FloralGroup.Infrastructure.Services;
using FloralGroup.WebApi.HealthChecks;
using FloralGroup.WebApi.MiddleWares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration["FloralGroupConnectionStrings:FileStorageConnection"];

builder.Services.AddDbContext<FilesDBContext>(options =>
    options.UseSqlServer(connectionString));
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
            RoleClaimType = ClaimTypes.Role, // must match token generation
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<ApplicationFileStorageService>();
builder.Services.AddHealthChecks()
    .AddCheck<FileHealthCheck>("filesystem");
//builder.Services.AddSwaggerGen();



var app = builder.Build();
app.UseMiddleware<TokenValidationMW>(builder.Configuration["Jwt:Key"]);
app.UseMiddleware<ExceptionHandlingMW>();
app.UseMiddleware<CorrelationWM>();
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.MapGet("/", () => "API is running successfully");
app.MapHealthChecks("/health");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
