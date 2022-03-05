using CRM.DataLayer.Repositories;
using CRM.DataLayer.Repositories.Interfaces;
using CRM.APILayer.Extensions;

var builder = WebApplication.CreateBuilder(args);

string _connectionStringVariableName = "CRM_CONNECTION_STRING";
string connString = builder.Configuration.GetValue<string>(_connectionStringVariableName);

builder.Services.Configure<BaseRepository>(opt =>
{
    opt.ConnectionString = connString;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterCRMRepositories();
builder.Services.RegisterCRMServices();
builder.Services.RegisterCRMAutomappers();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
