using FilmesAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Configurações de conexão do ENTITY FRAMEWORK com o MySQL ----------------------------->>>
var connectionString = builder.Configuration.GetConnectionString("FilmeConnection"); //captura a connectionstring armazenada no appsettings.json

builder.Services.AddDbContext<FilmeContext>(opts =>
    opts.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//-------------------------------------------------------------------------------------->>>

//Configurando o AutoMapper para todo o contexto da aplicação -------------------------->>>

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//-------------------------------------------------------------------------------------->>>

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
