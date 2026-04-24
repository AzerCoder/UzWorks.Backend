using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using UzWorks.API;
using UzWorks.API.Hubs;
using UzWorks.API.Middleware;
using UzWorks.API.Utils;
using UzWorks.BL;
using UzWorks.Core.Abstract;
using UzWorks.Core.AccessConfigurations;
using UzWorks.Identity;
using UzWorks.Infrastructure;
using UzWorks.Infrastructure.ExceptionHandling;
using UzWorks.Persistence;

// Must be set BEFORE CreateBuilder — prevents FileSystemWatcher creation
// which hits Linux inotify instance limit (1024) on Render free tier.
Environment.SetEnvironmentVariable("DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false");

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel();

builder.Services.AddOptions();
builder.Services.Configure<AccessConfiguration>(builder.Configuration.GetSection("AccessConfiguration"));
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    //options.DocumentFilter<PathPrefixInsertDocumentFilter>("api");

    options.SwaggerDoc("v1", new OpenApiInfo { Title = "UzWorks Api", Version = "v1" });
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });

});

builder.Services.AddScoped<IEnvironmentAccessor, EnvironmentAccessor>();

builder.Services.RegisterIdentityModule(builder.Configuration);

// Allow SignalR to read JWT from query string (?access_token=...)
builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(token) && path.StartsWithSegments("/hubs"))
                context.Token = token;
            return Task.CompletedTask;
        }
    };
});
builder.Services.RegisterPersistenceModule(builder.Configuration);
builder.Services.RegisterBLModule();
builder.Services.RegisterInfrastructureModule(builder.Configuration);

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, ChatUserIdProvider>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(opt =>
    {
        opt.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });

    options.AddPolicy("SignalRPolicy", opt =>
    {
        opt.WithOrigins(
                builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                ?? ["http://localhost:3000", "http://localhost:5173"])
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

await app.UseRoleInitializerMiddleware();
await app.UseLocationInitializerMiddleware();
await app.UseJobCategoryInitializerMiddleware();

//app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandler>();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat").RequireCors("SignalRPolicy");
app.MapGet("/", () => Results.Ok("UzWorks backend is running"));
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.Run();
