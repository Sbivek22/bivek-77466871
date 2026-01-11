using WebAdditonal.Models;
using WebAdditonal.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<BooseWebRunner>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();

// API endpoint: BOOSE code in, JSON out
//app.MapPost("/api/run", (RunRequest req, BooseWebRunner runner) =>
//{
//    var res = runner.Run(req);
//    return Results.Json(res);
//});

app.MapPost("/api/run", (RunRequest req, BooseWebRunner runner) =>
{
    try
    {
        return Results.Json(runner.Run(req));
    }
    catch (Exception ex)
    {
        return Results.Json(new RunResponse
        {
            Success = false,
            Output = "ERROR: " + ex.Message
        });
    }
});



app.Run();
