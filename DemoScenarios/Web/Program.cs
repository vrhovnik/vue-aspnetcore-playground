using System.Net;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Web.Helpers;
using Web.Options;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<DefaultWebOptions>(builder.Configuration
    .GetSection(OptionsHelper.DefaultWebOptions));
builder.Services.AddHealthChecks();
builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<CategoriesHttpService>();
builder.Services.AddHttpClient<GeneralHttpService>();
builder.Services.AddHttpClient<MemoryHttpService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
    options.Conventions.AddPageRoute("/Info/Index", ""));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(new CorsPolicyBuilder()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
        .Build());
});
var app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Info/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.MapRazorPages();
app.MapControllers();
app.UseExceptionHandler(options =>
{
    options.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";
        var exception = context.Features.Get<IExceptionHandlerFeature>();
        if (exception != null)
        {
            var message = $"{exception.Error.Message}";
            await context.Response.WriteAsync(message).ConfigureAwait(false);
        }
    });
});
app.MapHealthChecks($"/{RouteHelper.HealthApiRoute}", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();