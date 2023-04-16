using MPay.API.Endpoints;
using MPay.Core;
using MPay.Infrastructure;
using MPay.Infrastructure.Common;
using MPay.Infrastructure.Cors;
using MPay.Infrastructure.DAL;
using MPay.Infrastructure.ErrorHandling;
using MPay.Infrastructure.Events;
using MPay.Infrastructure.FeatureFlags;
using MPay.Infrastructure.Validation;
using MPay.Infrastructure.Webhooks;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddLogger(builder.Configuration);

builder.Services.ConfigureOptions(builder.Configuration);
builder.Services.ConfigureJson();
builder.Services.AddCommon();
builder.Services.AddCorsConfiguration();
builder.Services.AddFeatureFlags(builder.Configuration);
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
app.UseCors(builder.Configuration);
app.UseErrorHandling();

if (app.Environment.IsDevelopment()) app.UseSwagger();

app.UseHealthCheck();
app.MapAPIEndpoints();
app.MapPurchaseEndpoints();

app.Run();