using CRM_APILayer.Extensions;
using CRM_DataLayer.Repositories;
using CRM_DataLayer.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);
string _connectionStringVariableName = "CRM_CONNECTION_STRING";


string connectionString = "Server=.\\SQLEXPRESS;Initial Catalog=userstore;Integrated Security=True";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ILeadRepository, LeadRepository>(provider => new LeadRepository(connectionString));
builder.Services.RegisterCRMServices();
builder.Services.RegisterCRMRepositories();
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
