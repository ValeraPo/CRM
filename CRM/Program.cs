using CRM.APILayer.Extensions;
using CRM.APILayer.Infrastructure;
using CRM.DataLayer.Configuration;

var builder = WebApplication.CreateBuilder(args);
string _logDirectoryVariableName = "LOG_DIRECTORY";
string _connectionStringVariableName = "CRM_CONNECTION_STRING";
string _identityUrlVariableName = "IDENTITY_SERVICE_URL";
string connString = builder.Configuration.GetValue<string>(_connectionStringVariableName);
string logDirectory = builder.Configuration.GetValue<string>(_logDirectoryVariableName);
string identityUrl = builder.Configuration.GetValue<string>(_identityUrlVariableName);

builder.Services.Configure<DbConfiguration>(opt =>
{
    opt.ConnectionString = connString;
});

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

var app = builder.Build();

app.Configuration["identityUrl"] = identityUrl;

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ErrorExceptionMiddleware>();

app.MapControllers();

app.Run();
