using SGMCJ.Application.Interfaces.Service;
using SGMCJ.Application.Services;
using SGMCJ.Domain.Repositories.Appointments;
using SGMCJ.Persistence;
using SGMCJ.Persistence.Repositories;
using SGMCJ.Persistence.Repositories.Appointments;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddTransient<IAppointmentService, AppointmentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();