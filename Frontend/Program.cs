var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseRouting();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

Console.WriteLine("🌐 Frontend iniciado en http://localhost:5001");

app.Run("http://localhost:5001");
