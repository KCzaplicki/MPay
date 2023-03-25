var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEntityFramework(builder.Configuration);
builder.Services.AddSwagger();
builder.Services.AddHealthCheck();

var app = builder.Build();

app.UseMigrations();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseHealthCheck();
app.MapHomeEndpoints();

app.Run();
