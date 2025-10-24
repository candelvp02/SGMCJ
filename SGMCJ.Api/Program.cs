using Microsoft.EntityFrameworkCore;
using SGMCJ.Persistence.Context;
using SGMCJ.Infrastructure.Dependencies;

var builder = WebApplication.CreateBuilder(args);

//DbContext
builder.Services.AddDbContext<HealtSyncContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("HealtSyncConnection")));

// capa de application and persistence

builder.Services.AddUserDependencies();
builder.Services.AddDoctorDependencies();
builder.Services.AddPatientDependencies();
builder.Services.AddAppointmentDependencies();
builder.Services.AddAvailabilityDependencies();
builder.Services.AddInsuranceProviderDependencies();
builder.Services.AddMedicalRecordDependencies();
builder.Services.AddNotificationDependencies();
builder.Services.AddReportDependencies();
builder.Services.AddSpecialtyDependencies();

//controllers & swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//http configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
//app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();