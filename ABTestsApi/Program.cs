using ABTestsApi.Common.AuthenticationHandlers;
using ABTestsApi.Common.Constants;
using ABTestsApi.DataAccess;
using ABTestsApi.Models.Services;
using ABTestsApi.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAuthentication()
    .AddScheme<DeviceTokenAuthenticationOptions, DeviceTokenAuthenticationHandler>("DeviceTokenAuthenticationScheme", options =>
    {
        options.DeviceTokenQueryParamName = QueryParamNames.DeviceToken;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<DeviceTokenOperationFilter>();
});

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IBtnColorProviderService, BtnColorProviderService>();
builder.Services.AddSingleton<IPriceProviderService, PriceProviderService>();

builder.Services.AddScoped<IDatabaseManager, DatabaseManager>();

builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IExperimentRepository, ExperimentRepository>();
builder.Services.AddScoped<IExperimentOptionRepository, ExperimentOptionRepository>();
builder.Services.AddScoped<IDeviceExperimentOptionRepository, DeviceExperimentOptionRepository>();

builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IExperimentService, ExperimentService>();
builder.Services.AddScoped<IExperimentValueProviderService, ExperimentValueProviderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else 
{
    app.UseExceptionHandler(app =>
    {
        app.Run(context =>
        {
            context.Response.StatusCode = 500;
            return Task.CompletedTask;
        });
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
