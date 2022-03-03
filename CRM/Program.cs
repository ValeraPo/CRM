using CRM_DataLayer.Repositories;

var builder = WebApplication.CreateBuilder(args);


string connectionString = "Server=.\\SQLEXPRESS;Initial Catalog=userstore;Integrated Security=True";

builder.Services.AddTransient<IBaseRepository, BaseRepository>(provider => new BaseRepository(connectionString));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
