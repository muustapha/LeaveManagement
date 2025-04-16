using LeaveManagement.Models.Datas;
using Microsoft.EntityFrameworkCore;
using LeaveManagement.Models.Services;
using LeaveManagement.Validators;
var builder = WebApplication.CreateBuilder(args);

// 🔧 Services
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("LeaveDb"));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<LeaveRequestValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔧 Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leave Management API");
        c.RoutePrefix = string.Empty; // Swagger accessible à la racine "/"
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LeaveManagement.Models.Datas.AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();

