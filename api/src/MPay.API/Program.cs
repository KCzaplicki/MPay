var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOptions(builder.Configuration);
builder.Services.ConfigureJson();
builder.Services.AddEntityFramework(builder.Configuration);
builder.Services.AddUnitOfWork();
builder.Services.AddAutoMapper();
builder.Services.AddSwagger();
builder.Services.AddErrorHandling();
builder.Services.AddHealthCheck();
builder.Services.AddFactories();
builder.Services.AddValidation();
builder.Services.AddValidators();
builder.Services.AddRepositories();
builder.Services.AddPolicies();
builder.Services.AddServices();
builder.Services.AddBackgroundServices();

var app = builder.Build();

app.UseMigrations();
app.UseErrorHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseHealthCheck();
app.MapHomeEndpoints();
app.MapPurchaseEndpoints();

app.Run();
