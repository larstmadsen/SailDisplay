using Microsoft.AspNetCore.Hosting;
using SailDisplay.Components;
using SailDisplay.Components.Data;
using SailDisplay.Components.Hubs;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.UseUrls("http://*:5000;https://*:5001");
builder.WebHost.UseUrls("http://*:5000");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<NetService>();
builder.Services.AddScoped<BrowserService>(); // scoped service


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//app.MapBlazorHub();
app.MapHub<NetHub>("/nethub");



app.Run();
