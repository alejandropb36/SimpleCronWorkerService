using SimpleCronWorkerService;
using WebApplicationApiSample.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCronWorkerService<Worker>(options =>
{
    // Run every second
    options.CronExpression = @"@every_second";
    options.TimeZone = TimeZoneInfo.Local;
});

builder.Services.AddCronWorkerService<Worker2>(options =>
{
    // Run every 1 minutes
    options.CronExpression = @"*/1 * * * *";
    options.TimeZone = TimeZoneInfo.Local;
});

builder.Services.AddCronWorkerService<WorkerWithSemaphore>(options =>
{
    // Run every 2 seconds
    options.CronExpression = @"*/2 * * * * *";
    options.TimeZone = TimeZoneInfo.Local;
    options.CronExpressionIncludeSeconds = true;
});

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

app.UseAuthorization();

app.MapControllers();

app.Run();
