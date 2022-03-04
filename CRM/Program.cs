using CRM_APILayer.Extensions;

var builder = WebApplication.CreateBuilder(args);
string _connectionStringVariableName = "CRM_CONNECTION_STRING";


string connectionString = "Server=.\\SQLEXPRESS;Initial Catalog=userstore;Integrated Security=True";

builder.Services.AddTransient<IBaseRepository, BaseRepository>(provider => new BaseRepository(connectionString));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
