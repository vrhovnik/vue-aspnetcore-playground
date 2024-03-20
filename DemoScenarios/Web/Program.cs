using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Web.Helpers;
using Web.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<DefaultWebOptions>(builder.Configuration
    .GetSection(OptionsHelper.DefaultWebOptions));
builder.Services.AddHealthChecks();
builder.Services.AddHealthChecksUI().AddInMemoryStorage();

builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
    options.Conventions.AddPageRoute("/Info/Index", ""));

var app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/" + RouteHelper.HealthApiRoute, new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }).AllowAnonymous();
    endpoints.MapRazorPages();
    endpoints.MapControllers();
    endpoints.MapHealthChecksUI();
});

app.Run();