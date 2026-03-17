using CIC.DataUtility;
using CIC.Helper;
using CIC.Model;
using CIC_Services;
using CIC_Services.Interfaces;
using CIC_Services.Services;
using JC.Criff.Highmark;
using JC.Experian;
using JC.Experian.Interfaces;
using JC.TransUnion.Cibil;
using JC.TransUnion.Cibil.Interface;
using JC.TransUnion.Cibil.Services;
using LoggerLibrary;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Serialization;
using NLog;

IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json").AddJsonFile("IpValidation.json", optional: true, reloadOnChange: true)
                            .Build();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("IpValidation.json", optional: false, reloadOnChange: true);
builder.Services.Configure<LogCleanerSettings>(builder.Configuration.GetSection("LogCleanerSettings"));
//logger
//logger
LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
var MyAllowSpecificOrigine = "_MyAllowSpecificOrigine";

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
});

//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.PropertyNamingPolicy = new UpperCaseNamingPolicy();
//    });

builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<AppSettingModel>(configuration);
builder.Services.AddHostedService<LogCleanerBackgroundService>();
builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Services.AddScoped<IExperianService, ExperianService>();
builder.Services.AddHttpClient<IExperianSoapClient, ExperianSoapClient>();

builder.Services.AddScoped<ITransunionCibilService, TransunionCibilService>();
builder.Services.AddScoped<ICibilService, CibilService>();

builder.Services.AddScoped<ICrifService, CrifService>();
builder.Services.AddScoped<ICirffServiceApp, CirffServiceApp>();

builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.AddSingleton<FileService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigine,
         policy =>
         {
             policy//.WithOrigins("http://localhost:5173/application", "http://localhost:5173/")
             .AllowAnyHeader()
             .AllowAnyMethod()
             .AllowAnyOrigin();
         });
});
var app = builder.Build();

var loggerFactory = app.Services.GetService<ILoggerFactory>();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Experian_Report")
    ),
    RequestPath = "/experian_report"   // ✅ public URL prefix
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Crif_Report")
    ),
    RequestPath = "/crif_report"   // ✅ public URL prefix
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CIC Services V1");
    c.RoutePrefix = "swagger"; // default
});
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();
//app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
