using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using MinimalApi.Data;
using MinimalApi.Data.Entity;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using HealthChecks.UI.Client;
using MinimalApi.Core; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens; 
using MinimalApi.Endpoints;
using MinimalApi.Services.Interfaces;
using MinimalApi.Services;

var builder = WebApplication.CreateBuilder(args); 
 
//heath check
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection") ?? "");

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSwaggerGen(option => {
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                }); 
});

builder.Services
    .AddScoped<IUserService, UserService>();
builder.Services
    .AddScoped<IEmailService, EmailService>();
builder.Services
    .AddDbContext<ApplicationDbContext>(option => option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services
    .AddAutoMapper(typeof(MappingConfig));
builder.Services
    .AddValidatorsFromAssemblyContaining<Program>();
builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("ApiSettings:Secret") ?? "SECRET_KEY")), 
            ValidateIssuer = false,
            ValidateAudience = false

        };
    });


builder.Services.AddAuthorization();
var app = builder.Build(); 
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}) ; 

app.UseAuthentication();
app.UseAuthorization();  

app.ConfigureUserEndpoints();
 
 
app.Run();
 