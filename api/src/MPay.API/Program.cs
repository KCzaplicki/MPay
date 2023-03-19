var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.MapHomeEndpoints();

app.Run();
