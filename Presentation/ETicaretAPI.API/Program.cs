using ETicaretAPI.API.Configurations.ColumWriters;
using ETicaretAPI.API.Extensions;
using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Infrastructure.Services.Storage.Azure;
using ETicaretAPI.Infrastructure.Services.Storage.Local;
using ETicaretAPI.Persistence;
using ETicaretAPI.SignalR;
using ETicaretAPI.SignalR.Hubs;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Logger log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration["Seq:ServerUrl"])
    .WriteTo.File("logs/log.txt")
    .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"),"logs", 
    columnOptions: new Dictionary<string, ColumnWriterBase> 
    {
        {"message", new RenderedMessageColumnWriter()},
        {"message_template", new MessageTemplateColumnWriter()},
        {"level", new LevelColumnWriter()},
        {"timestamp", new TimestampColumnWriter()},
        {"exception", new ExceptionColumnWriter()},
        {"log_event", new LogEventSerializedColumnWriter()},
        {"user_name", new UsernameColumnWriter()},
    }, needAutoCreateTable:true).Enrich.FromLogContext().MinimumLevel.Information().CreateLogger();
builder.Host.UseSerilog(log);

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

// Add services to the container.
builder.Services.AddHttpContextAccessor();// clienttan gelen istek neticesinde oluþturulan httpcontext nesnesine katmanlardaki sýnýflar üzerinden eriþebilmemizi saðlayan servis
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddSignalRServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddStorage<AzureStorage>();
builder.Services.AddApplicationServices();

builder.Services.AddCors(opt => opt.AddDefaultPolicy(policy => 
    policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()
));

builder.Services.AddControllers(opt => opt.Filters.Add<ValidationFilter>())
    .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
    .ConfigureApiBehaviorOptions(opt => opt.SuppressModelStateInvalidFilter = true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin", opt =>
    {
        opt.TokenValidationParameters = new()
        {
            ValidateAudience = true,    
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer= builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false, /*expires != null && expires > DateTime.UtcNow*/
            //ClockSkew = TimeSpan.FromMinutes(0),
            NameClaimType = ClaimTypes.Name 
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.ConfigureExceptionHandler(app.Services.GetRequiredService<ILogger<Program>>());

app.UseSerilogRequestLogging();

app.UseHttpLogging();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User?.Identity?.Name : null;
    var email = context.User?.FindFirst(ClaimTypes.Email)?.Value;
    LogContext.PushProperty("user_name", username);
    await next();
});

app.MapControllers();

app.MapHubs();

app.Run();
