var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEntityFramework(builder.Configuration);
builder.Services.AddAutoMapper();
builder.Services.AddSwagger();
builder.Services.AddHealthCheck();
builder.Services.AddValidators();
builder.Services.AddRepositories();
builder.Services.AddServices();

var app = builder.Build();

app.UseMigrations();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseHealthCheck();
app.MapHomeEndpoints();
app.MapPurchaseEndpoints();

app.Run();
