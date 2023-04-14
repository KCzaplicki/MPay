var builder = WebApplication.CreateBuilder(args);

builder.Host.AddLogger(builder.Configuration);

builder.Services.ConfigureOptions(builder.Configuration);
builder.Services.ConfigureJson();
builder.Services.AddCommon();
builder.Services.AddEntityFramework(builder.Configuration);
builder.Services.AddUnitOfWork();
builder.Services.AddAutoMapper();
builder.Services.AddSwagger();
builder.Services.AddErrorHandling();
builder.Services.AddValidationErrorsHandling();
builder.Services.AddHealthCheck();
builder.Services.AddWebhooks(builder.Configuration);
builder.Services.AddEventsBackgroundHandling();
builder.Services.AddEvents();
builder.Services.AddFactories();
builder.Services.AddValidators();
builder.Services.AddRepositories();
builder.Services.AddPolicies();
builder.Services.AddServices();
builder.Services.AddBackgroundServices();

var app = builder.Build();

app.UseLogger();
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
