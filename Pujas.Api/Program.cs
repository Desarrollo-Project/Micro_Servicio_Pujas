using MassTransit;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Pujas.Api;
using Pujas.Api.Controllers;
using Pujas.Dominio.Repositorios;
using Pujas.Infraestructura.Event_Bus.Consumidores;
using Pujas.Infraestructura.Persistencia;
using Pujas.Infraestructura.Persistencia.Repositorios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// Servicio para los web sockets SignalR

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendAccess",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        });
});

builder.Services.AddHttpClient("SubastasClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5247/api/Subastas/");
});

builder.Services.AddHttpClient("PujasClient", client =>
{
    // Establece la BaseAddress para la API de Pujas
    client.BaseAddress = new Uri("http://localhost:5029/"); // Puerto de la API de Pujas
});



// Configuracion para las bases de datos en Postgres 
var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<App_Db_Context>(options =>
    options.UseNpgsql(connectionString)
);

// Configuracion de base de datos en  MongoDb 
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDB")));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    return mongoClient.GetDatabase("Pujas_db");
});


// Configuracion Mass Transit 

builder.Services.AddMassTransit(x =>
{
    //Credenciales de rabbitMq
    var rabbitMq_Host = builder.Configuration["RabbitMQ:Host"];
    var rabbitMq_Username = builder.Configuration["RabbitMQ:Username"];
    var rabbitMq_Password = builder.Configuration["RabbitMQ:Password"];

    //Consumidores 
    x.AddConsumer<Crear_Puja_Consumidor>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMq_Host, h =>
        {
            h.Username(rabbitMq_Username);
            h.Password(rabbitMq_Password);
        });

        cfg.ReceiveEndpoint(("Crear_Puja_Event_Cola"), e =>
            { e.ConfigureConsumer<Crear_Puja_Consumidor>(context);   });
        
        cfg.ConfigureEndpoints(context);

    });

});


// Configuracion de Repositorios
builder.Services.AddScoped<IRepositorio_Pujas, Pujas_Repositorio>();
builder.Services.AddScoped<IRepositorio_Pujas_Lectura, Pujas_Repositorio_Lectura>();
builder.Services.AddHostedService<Temporizador>();

// Configuracion de MediatR para manejar comandos y consultas
var applicationAssembly = typeof(Pujas.Aplicacion.Commands.Crear_Puja_Command).Assembly;
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowFrontendAccess");

app.MapHub<Pujas_Hub>("/hub/pujas", options => {
    options.Transports = HttpTransportType.WebSockets |
                         HttpTransportType.LongPolling;
});

app.MapControllers();

app.Run();
