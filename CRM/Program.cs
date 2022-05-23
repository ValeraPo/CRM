using CRM.APILayer.Extensions;
using CRM.APILayer.Infrastructure;
using CRM.BusinessLayer;
using CRM.DataLayer.Configuration;
using Marvelous.Contracts.Enums;

var builder = WebApplication.CreateBuilder(args);
// Reading from environment variable
string _logDirectoryVariableName = "LOG_DIRECTORY";
string _connectionStringVariableName = "CRM_CONNECTION_STRING";
string _identityUrlVariableName = "IDENTITY_SERVICE_URL";
string _configsUrlVariableName = "CONFIGS_SERVICE_URL";
string connString = builder.Configuration.GetValue<string>(_connectionStringVariableName);
string logDirectory = builder.Configuration.GetValue<string>(_logDirectoryVariableName);
string auth = builder.Configuration.GetValue<string>(_identityUrlVariableName);
var configs = builder.Configuration.GetValue<string>(_configsUrlVariableName);

// connecting data base
builder.Services.Configure<DbConfiguration>(opt =>
{
    opt.ConnectionString = connString;
});

//Setting logging
var config = new ConfigurationBuilder()
           .SetBasePath(logDirectory)
           .AddXmlFile("NLog.config", optional: true, reloadOnChange: true)
           .Build();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterSwaggerGen();

builder.Services.AddCustomAuth();

builder.Services.RegisterCRMRepositories();
builder.Services.RegisterCRMServices();
builder.Services.RegisterCRMAutomappers();
builder.Services.RegisterLogger(config);
builder.Services.AddMassTransit();
builder.Services.AddFluentValidation();

var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ErrorExceptionMiddleware>();

app.MapControllers();

//Setting configs for interaction with config service and authorization service
app.Configuration[Microservice.MarvelousConfigs.ToString()] = configs;
app.Configuration[Microservice.MarvelousAuth.ToString()] = auth;

// Getting configs from config service
await app.Services.CreateScope().ServiceProvider.GetRequiredService<IInitializationHelper>().InitializeConfigs();


app.Run();
