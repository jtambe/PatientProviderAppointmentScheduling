using PatientProviderSchedulingApi.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.UseAllOfForInheritance();
    // Use annotations from the Swashbuckle.AspNetCore.Annotations package
    options.EnableAnnotations();
});
builder.Services.AddAppDependencies();
builder.Services.AddLogging(b =>
{
    b.AddConsole(); // Add the Console logger
});

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
